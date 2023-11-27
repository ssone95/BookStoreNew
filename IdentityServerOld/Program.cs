// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServerOld.Data.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using IdentityServerOld.Data.Domain;
using IdentityServerOld.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using IdentityServer4.Configuration;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Collections.Generic;

namespace IdentityServer
{
    public class Program
    {
        private static readonly string MigrationsAssemblyName = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
        private const string ApiCorsPolicy = "ApiCorsPolicy";
        private const string TokenExpirationTimeSpanFormat = @"h\:mm\:ss";

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .CreateLogger();

            try
            {
                Log.Information("Starting IdentityServer...");
                var builder = WebApplication.CreateBuilder(args);
                ConfigureBase(builder);
                ConfigureIdentityServer(builder);

                var app = builder.Build();
                await ConfigureApp(app);
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "IdentityServer terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureBase(WebApplicationBuilder builder)
        {
            builder.Services.AddLogging(options =>
            {
                options.ClearProviders();
                options.AddSerilog();
            });

            // Enabled for auth purposes
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                var connString = builder.Configuration.GetConnectionString("UsersDb");
                options.UseSqlServer(connString, sql =>
                {
                    sql.EnableRetryOnFailure();
                    sql.MigrationsAssembly(MigrationsAssemblyName);
                });
                //options.UseSqlite(connString, sql => sql.MigrationsAssembly(MigrationsAssemblyName));
            });

            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        private static void ConfigureIdentityServer(WebApplicationBuilder webAppBuilder)
        {
            webAppBuilder.Host.UseSerilog((context, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });

			var builder = webAppBuilder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                options.UserInteraction = new UserInteractionOptions
                {
                    LogoutUrl = "/Account/Logout", // TODO: Implement Logout support
                    LoginUrl = "/Account/Login",
                    LoginReturnUrlParameter = "returnUrl"
                };
            }).AddAspNetIdentity<AppUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = db =>
                    {
                        db.EnableSensitiveDataLogging(true);
                        db.EnableDetailedErrors(true);
                        var connString = webAppBuilder.Configuration.GetConnectionString("ConfigurationDb");
						db.UseSqlServer(connString, so =>
						{
							so.EnableRetryOnFailure();
							so.MigrationsAssembly(MigrationsAssemblyName);
						});
						//db.UseSqlite(connString, sql => sql.MigrationsAssembly(MigrationsAssemblyName));
                    };
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = db =>
                    {
                        var connString = webAppBuilder.Configuration.GetConnectionString("OperationalDb");
						db.UseSqlServer(connString, sql =>
						{
							sql.EnableRetryOnFailure();
							sql.MigrationsAssembly(MigrationsAssemblyName);
						});
						//db.UseSqlite(connString, sql => sql.MigrationsAssembly(MigrationsAssemblyName));
					};
                    options.EnableTokenCleanup = true;
                    var tokenExpirationTimeSpanStr = webAppBuilder.Configuration.GetValue<string>("Security:TokenExpirationTimeSpan");

                    if (string.IsNullOrEmpty(tokenExpirationTimeSpanStr) || !TimeSpan.TryParseExact(tokenExpirationTimeSpanStr, TokenExpirationTimeSpanFormat, CultureInfo.InvariantCulture, out TimeSpan tokenExpirationSpan))
                        throw new Exception($"Token Expiration (Security:TokenExpirationTimeSpan) must be set, and must be in the following format: {TokenExpirationTimeSpanFormat}!");

                    options.TokenCleanupInterval = (int)tokenExpirationSpan.TotalSeconds;
                });

            if (webAppBuilder.Environment.IsDevelopment() || webAppBuilder.Environment.IsEnvironment("Docker")) // Added Docker to avoid manual certificate creation
            {
                builder.AddDeveloperSigningCredential();
            }
        }

        private static async Task ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.None,
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });

            app.UseCors(ApiCorsPolicy);

            app.UseIdentityServer();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

			await app.MigrateAndSeedDb(app.Configuration);
        }
    }
}