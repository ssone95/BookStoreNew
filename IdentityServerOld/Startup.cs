// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        private const string ApiCorsPolicy = "ApiCorsPolicy";
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            //services.AddControllersWithViews();

            

            // services.AddCors(c =>
            // {
            //     c.AddPolicy(name: ApiCorsPolicy, b =>
            //     {
            //         b.WithOrigins("http://localhost:6000", "https://localhost:6001");
            //     });
            // });

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
                options.IssuerUri = "http://localhost:5000";
            })
                // .AddInMemoryIdentityResources(Config.IdentityResources)
                // .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddTestUsers(Config.TestUsers)
                .AddInMemoryClients(Config.Clients);

            // var builder = services.AddIdentityServer()
            //     .AddInMemoryApiScopes(Config.ApiScopes)
            //     .AddInMemoryClients(Config.Clients);

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
            // app.UseStaticFiles();
            // app.UseRouting();

            // app.UseCors(ApiCorsPolicy);
            
            app.UseIdentityServer();//.UseAuthentication();

            // uncomment, if you want to add MVC
            // app.UseAuthorization();
            // app.UseEndpoints(endpoints =>
            // {
            //    endpoints.MapDefaultControllerRoute();
            // });
        }
    }
}
