using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerOld.Data.ConfigurationModels;
using IdentityServerOld.Data.Contexts;
using IdentityServerOld.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerOld.Extensions
{
	public static class DbMigrationsHelper
    {
        public static async Task MigrateAndSeedDb(this IHost host, IConfiguration configuration)
		{
			var seedRoles = configuration.GetSection("Security:SeedRoles").Get<List<string>>();
			var seedUsers = configuration.GetSection("Security:SeedUsers").Get<List<AppUserConfig>>();
			var apiScopes = configuration.GetSection("Security:ApiScopes").Get<List<OAuthApiScope>>();
			var allowedClients = configuration.GetSection("Security:AllowedClients").Get<List<OAuthClient>>();
			using var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

            await MigrateAndSeedPersistedGrantDb(scope);
            await MigrateAndSeedAppDb(scope, seedRoles, seedUsers);
            await MigrateAndSeedConfigurationDb(scope, apiScopes, allowedClients);

        }

        private static async Task MigrateAndSeedConfigurationDb(IServiceScope scope, List<OAuthApiScope> apiScopes, List<OAuthClient> allowedClients)
        {
            using var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            await configurationDbContext.Database.MigrateAsync();

			if (configurationDbContext.ApiScopes.Count() < 1)
			{
				List<ApiScope> realScopes = MapToIdentityApiScopes(apiScopes);
				configurationDbContext.ApiScopes.AddRange(realScopes);
				configurationDbContext.SaveChanges();
			}

			if (configurationDbContext.Clients.Count() < 1)
			{
				IEnumerable<Client> realClients = MapToIdentityClients(allowedClients);
				configurationDbContext.Clients.AddRange(realClients);
				configurationDbContext.SaveChanges();
			}
		}

		private static List<ApiScope> MapToIdentityApiScopes(List<OAuthApiScope> apiScopes)
		{
			return apiScopes
				.Select(x => new ApiScope()
				{
					Name = x.Name,
					Description = x.Description,
					DisplayName = x.DisplayName,
					Enabled = x.Enabled,
					ShowInDiscoveryDocument = x.ShowInDiscoveryDocument
				})
				.ToList();
		}

		private static IEnumerable<Client> MapToIdentityClients(List<OAuthClient> allowedClients)
		{
			return allowedClients.Select(x => new Client()
			{
				ClientId = x.ClientId,
				ClientName = x.ClientName,
				AllowAccessTokensViaBrowser = x.AllowAccessTokensViaBrowser,
				RequireConsent = x.RequireConsent,
				EnableLocalLogin = x.EnableLocalLogin,
				Enabled = x.Enabled,

				AllowedGrantTypes = x.AllowedGrantTypes
					?.Select(y => new ClientGrantType() { GrantType = y }).ToList() ?? new List<ClientGrantType>(),
				AllowedCorsOrigins = x.AllowedCorsOrigins
					?.Select(y => new ClientCorsOrigin() { Origin = y }).ToList() ?? new List<ClientCorsOrigin>(),
				Claims = x.Claims
					?.Select(y => new ClientClaim() { Type = y.Type, Value = y.Value }).ToList() ?? new List<ClientClaim>(),
				ClientSecrets = x.ClientSecrets
					?.Select(y
						=> new ClientSecret()
						{
							Created = DateTime.Now,
							Description = y.Description,
							Value = IdentityServer4.Models.HashExtensions.Sha256(y.Value),
							Type = y.Type
						}).ToList() ?? new List<ClientSecret>(),
				RedirectUris = x.RedirectUris
					?.Select(y => new ClientRedirectUri() { RedirectUri = y }).ToList() ?? new List<ClientRedirectUri>(),
				PostLogoutRedirectUris = x.PostLogoutRedirectUris
					?.Select(y => new ClientPostLogoutRedirectUri() { PostLogoutRedirectUri = y }).ToList() ?? new List<ClientPostLogoutRedirectUri>(),
				AllowedScopes = x.AllowedScopes
					?.Select(y => new ClientScope() { Scope = y }).ToList() ?? new List<ClientScope>()
			});
		}

		private static async Task MigrateAndSeedAppDb(IServiceScope scope, List<string> seedRoles, List<AppUserConfig> seedUsers)
        {
            using var userContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            await userContext.Database.MigrateAsync();

            if (userContext.Roles.Count() < 1)
            {
                foreach (string role in seedRoles)
                {
                    var roleStore = new RoleStore<IdentityRole>(userContext);
                    var result = roleStore.CreateAsync(new IdentityRole(role)
                    {
                        NormalizedName = role.ToUpper()
                    }).Result;
                    Log.Information("Role {RoleName} created!", role);
                }
            }

            if (userContext.Users.Count() < 1)
            {
                var passwordHasher = new PasswordHasher<AppUser>();
				var userStore = new UserStore<AppUser>(userContext);
				seedUsers.ForEach(x =>
                {
                    var dbUser = new AppUser()
                    {
                        UserName = x.UserName,
                        NormalizedUserName = x.NormalizedUserName.ToUpper(),
                        Email = x.Email,
                        NormalizedEmail = x.NormalizedEmail.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                        EmailConfirmed = x.EmailConfigured,
                        PhoneNumberConfirmed = x.PhoneNumberConfirmed
                    };

					dbUser.PasswordHash = passwordHasher.HashPassword(dbUser, x.Password);
					var result = userStore.CreateAsync(dbUser).Result;

					AppUser existingUser = userManager.FindByEmailAsync(dbUser.Email).Result;
					var roleAssignmentResult = userManager.AddToRolesAsync(dbUser, x.Roles).Result;
					Log.Information("Created user {UserName}, status: {Result}, roleAssignmentResult: {RoleAssignmentResult}", x.UserName, result, roleAssignmentResult);
				});

            }
        }

        private static async Task MigrateAndSeedPersistedGrantDb(IServiceScope scope)
        {
            using var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

            await persistedGrantDbContext.Database.MigrateAsync();
        }
    }
}
