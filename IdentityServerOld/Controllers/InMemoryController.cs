using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerOld.Controllers
{
    [AllowAnonymous]
    public class InMemoryController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        public InMemoryController(ConfigurationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
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
