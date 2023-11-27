using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerOld.Controllers
{
    [AllowAnonymous]
    public class InMemoryController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger<InMemoryController> _logger;

        public InMemoryController(ConfigurationDbContext dbContext, ILogger<InMemoryController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Entering {0} method...", nameof(Index));
            var clients = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Claims)
                .Include(x => x.ClientSecrets)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .ToListAsync();

            var apiScopes = await _dbContext.ApiScopes
                .ToListAsync();
            return View(new InMemoryData()
            {
                Clients = clients,
                ApiScopes = apiScopes
            });
        }
    }

    public class InMemoryData
    {
        public List<Client> Clients { get; set; }
        public List<ApiScope> ApiScopes { get; set; }
    }
}
