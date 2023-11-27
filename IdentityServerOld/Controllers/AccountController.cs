using IdentityServer4.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System;
using IdentityServerOld.Data.Models;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using IdentityServerOld.Data.Domain;
using IdentityServerOld.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServerOld.Controllers
{
    [AllowAnonymous]
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, 
            IIdentityServerInteractionService interaction, IClientStore clientStore)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var vm = PrepareLoginFormViewModel(returnUrl: returnUrl);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModelBase model)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please check your inputs!");
                var vm = PrepareLoginFormViewModel(model, errorMessage: "A validation error occurred, please check errors below:TODO!!!");
                return View(vm);
            }
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid account entered!");
                var vm = PrepareLoginFormViewModel(model, errorMessage: "A validation error occurred, please check errors below:TODO!!!");
                return View(vm);
            }

            if (context != null)
            {
                if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                }

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(model.ReturnUrl);
            }

            var user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid account entered!");
                var vm = PrepareLoginFormViewModel(model, errorMessage: "A validation error occurred, please check errors below:TODO!!!");
                return View(vm);
            }

            if (context != null)
            {
                return Redirect(context.RedirectUri);
            }

            if (Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

			ModelState.AddModelError(string.Empty, "No url was configured to serve as return endpoint...");
			var errorVm = PrepareLoginFormViewModel(model, errorMessage: "No url was configured to serve as return endpoint...");
			return View(errorVm);
		}
        private LoginFormViewModel PrepareLoginFormViewModel(string returnUrl)
        {
            return new LoginFormViewModel()
            {
                ReturnUrl = returnUrl,
            };
        }

        private LoginFormViewModel PrepareLoginFormViewModel(LoginViewModelBase previousInputs, string? errorMessage = null)
        {
            return new LoginFormViewModel()
            {
                ReturnUrl = previousInputs.ReturnUrl,
                Username = previousInputs?.Username,
                Password = previousInputs?.Password,
                ErrorMessage = errorMessage
            };
        }
    }

    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            //if (result is ViewResult)
            //{
            //    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
            //    if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
            //    {
            //        context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            //    }

            //    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
            //    if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
            //    {
            //        context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            //    }

            //    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
            //    var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';";
            //    // also consider adding upgrade-insecure-requests once you have HTTPS in place for production
            //    //csp += "upgrade-insecure-requests;";
            //    // also an example if you need client images to be displayed from twitter
            //    // csp += "img-src 'self' https://pbs.twimg.com;";

            //    // once for standards compliant browsers
            //    if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
            //    {
            //        context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
            //    }
            //    // and once again for IE
            //    if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
            //    {
            //        context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
            //    }

            //    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
            //    var referrer_policy = "no-referrer";
            //    if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
            //    {
            //        context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
            //    }
            //}
        }
    }
}
