// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerOld.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using IdentityServerOld.Data.Domain;
using IdentityModel;
using System.Security.Claims;
using static IdentityServer4.Models.IdentityResources;
using System.Data;

namespace IdentityServer
{
    public class Program
    {
        public static string Sha256(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha.ComputeHash(bytes);

                return Convert.ToBase64String(hash);
            }
        }

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Verbose)
                .MinimumLevel.Override("System", LogEventLevel.Verbose)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Verbose)
                .MinimumLevel.Override("IdentityServer4.Validation", LogEventLevel.Verbose)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                Log.Information("Starting host...");
                string original = "Secret123$";
                string hashed = Sha256(original);
                Log.Information("Hashed secret: {original} -> {hashed}", original, hashed);
                var builder = CreateHostBuilder(args).Build();
                using (var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>())
                    {
                        persistedGrantDbContext.Database.Migrate();
                    }
                }
                using (var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var userContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        userContext.Database.Migrate();
                    }
                }
                using (var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>())
                    {
                        configurationDbContext.Database.Migrate();
                    }
                }
                using (var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>())
                    using (var userContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                    {
                        string[] roles = new string[] { "Administrator", "User" };
                        if (userContext.Roles.Count() < 1)
                        {
                            foreach (string role in roles)
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
                            var users = Config.TestUsers
                                .Select(x
                                    => new AppUser()
                                    {
                                        UserName = "test123",
                                        NormalizedUserName = "test123".ToUpper(),
                                        Email = "nedeljko.savic.c@gmail.com",
                                        NormalizedEmail = "nedeljko.savic.c@gmail.com".ToUpper(),
                                        ConcurrencyStamp = Guid.NewGuid().ToString("D"),
                                        EmailConfirmed = true,
                                        PhoneNumberConfirmed = true
                                    }).ToList();

                            var userStore = new UserStore<AppUser>(userContext);
                            users.ForEach(user =>
                            {
                                user.PasswordHash = passwordHasher.HashPassword(user, "Secret123$");
                                var result = userStore.CreateAsync(user).Result;

                                AppUser existingUser = userManager.FindByEmailAsync(user.Email).Result;
                                var roleAssignmentResult = userManager.AddToRolesAsync(user, roles).Result;
                                Log.Information("Created user {UserName}, status: {Result}, roleAssignmentResult: {RoleAssignmentResult}", user.UserName, result, roleAssignmentResult);
                            });

                        }
                    }
                }
                using (var scope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    using (var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>())
                    {
                        if (configurationDbContext.Clients.Count() < 1)
                        {
                            var realClients = Config.Clients.Select(x => new Client()
                            {
                                ClientId = x.ClientId,
                                ClientName = x.ClientName,

                                AllowedGrantTypes = x.AllowedGrantTypes
                                    .Select(y
                                        => new ClientGrantType()
                                        {
                                            GrantType = y
                                        }).ToList(),
                                AllowAccessTokensViaBrowser = x.AllowAccessTokensViaBrowser,
                                RequireConsent = x.RequireConsent,
                                EnableLocalLogin = x.EnableLocalLogin,
                                Enabled = x.Enabled,
                                AllowedCorsOrigins = x.AllowedCorsOrigins
                                    .Select(y
                                        => new ClientCorsOrigin()
                                        {
                                            Origin = y
                                        }).ToList(),

                                Claims = x.Claims
                                    .Select(y
                                        => new ClientClaim()
                                        {
                                            Type = y.Type,
                                            Value = y.Value
                                        }).ToList(),

                                ClientSecrets = x.ClientSecrets
                                    .Select(y
                                        => new ClientSecret()
                                        {
                                            Created = DateTime.Now,
                                            Description = y.Description,
                                            Value = y.Value,
                                            Type = y.Type
                                        }).ToList(),

                                RedirectUris = x.RedirectUris
                                    .Select(y
                                        => new ClientRedirectUri()
                                        {
                                            RedirectUri = y
                                        }).ToList(),
                                PostLogoutRedirectUris = x.PostLogoutRedirectUris
                                    .Select(y
                                        => new ClientPostLogoutRedirectUri()
                                        {
                                            PostLogoutRedirectUri = y
                                        }).ToList(),

                                AllowedScopes = x.AllowedScopes
                                    .Select(y
                                        => new ClientScope()
                                        {
                                            Scope = y
                                        }).ToList()

                            });
                            configurationDbContext.Clients.AddRange(realClients);
                            configurationDbContext.SaveChanges();
                        }

                        if (configurationDbContext.ApiScopes.Count() < 1)
                        {
                            var apiScopes = Config.ApiScopes
                                .Select(x => new ApiScope()
                                {
                                    Name = x.Name,
                                    Description = x.Description,
                                    DisplayName = x.DisplayName,
                                    Enabled = x.Enabled,
                                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument
                                })
                                .ToList();
                            configurationDbContext.ApiScopes.AddRange(apiScopes);
                            configurationDbContext.SaveChanges();
                        }
                    }
                }
                builder.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}