// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Configuration;
using IdentityServerOld.Data.Contexts;
using IdentityServerOld.Data.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace IdentityServer
{
    public class Startup
    {
        private const string ApiCorsPolicy = "ApiCorsPolicy";
        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();



            // services.AddCors(c =>
            // {
            //     c.AddPolicy(name: ApiCorsPolicy, b =>
            //     {
            //         b.WithOrigins("http://localhost:6000", "https://localhost:6001");
            //     });
            // });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                var connString = Configuration.GetConnectionString("Users");
                options.UseSqlite(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            });
                //options.UseSqlite("Data Source=AspIdUsers.db;"));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //var builder = services.AddIdentityServer(options =>
            //{
            //    // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
            //    options.EmitStaticAudienceClaim = true;
            //    options.IssuerUri = "http://localhost:5000";
            //})
            //    .AddAspNetIdentity<AppUser>()
            //    // .AddInMemoryIdentityResources(Config.IdentityResources)
            //    // .AddInMemoryApiResources(Config.ApiResources)
            //    .AddInMemoryApiScopes(Config.ApiScopes)
            //    .AddTestUsers(Config.TestUsers)
            //    .AddInMemoryClients(Config.Clients);
            //// var builder = services.AddIdentityServer()
            ////     .AddInMemoryApiScopes(Config.ApiScopes)
            ////     .AddInMemoryClients(Config.Clients);


            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction = new UserInteractionOptions
                {
                    LogoutUrl = "/Account/Logout",
                    LoginUrl = "/Account/Login",
                    LoginReturnUrlParameter = "returnUrl"
                };
            })
                .AddAspNetIdentity<AppUser>()
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = db =>
                    {
                        db.EnableSensitiveDataLogging(true);
                        db.EnableDetailedErrors(true);
                        var connString = Configuration.GetConnectionString("Configuration");
                        db.UseSqlite(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = db =>
                    {
                        var connString = Configuration.GetConnectionString("Operational");
                        db.UseSqlite(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    // options.TokenCleanupInterval = 15; // interval in seconds. 15 seconds useful for debugging
                });


            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            // services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.None,
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });

            app.UseCors(ApiCorsPolicy);

            app.UseIdentityServer();//.UseAuthentication();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
