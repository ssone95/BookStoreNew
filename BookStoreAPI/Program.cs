using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

internal class Program
{
    private const string IdentityServerCorsPolicy = "IdentityServerCorsPolicy";

    private static void Main(string[] args)
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
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddLogging();
            builder.Services.AddSerilog(logger: Log.Logger);
            builder.Services.AddHttpLogging(c =>
            {
                c.LoggingFields = HttpLoggingFields.All;
            });

            builder.Services.AddCors(c =>
            {
                c.AddPolicy(name: IdentityServerCorsPolicy, b =>
                {
                    b.WithOrigins("http://localhost:5000", "http://localhost:5001")
                        .AllowCredentials().AllowAnyHeader().AllowAnyMethod();
                });
            });
            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => 
            {
                // c.SwaggerDoc("v1", new OpenApiInfo()
                // {
                //     Title = "",
                //     Description = "Something here in description..."
                // });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow() {
                            AuthorizationUrl = new Uri("http://localhost:5000/connect/authorize"),
                            Scopes = { { "bookStoreApi", "Book Store Implicit Flow" } }
                        },
                        ClientCredentials = new OpenApiOAuthFlow()
                        {
                            TokenUrl = new Uri("http://localhost:5000/connect/token"),
                            Scopes = { { "bookStoreApiPostman", "Book Store ClientCredentials Flow" } },
                        }
                    },
                    OpenIdConnectUrl = new Uri("http://localhost:5000/"),
                    Scheme = "Bearer",
                    Name = "oauth2",
                });
            });
            builder.Services.AddControllers();
            builder.Services.AddAuthentication("IdentityServer")
                .AddJwtBearer("Bearer", o => 
                {
                    o.Authority = "http://localhost:5000";
                    o.TokenValidationParameters = new TokenValidationParameters() 
                    {
                        ValidateAudience = false,
                    };
                    // Disabling for local development purposes
                    o.RequireHttpsMetadata = false;
                })
                .AddIdentityServerAuthentication("IdentityServer", o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    // o.ApiName = "bookStore";
                    // o.SupportedTokens = IdentityServer4.AccessTokenValidation.SupportedTokens.Jwt;
                    // o.Challenge = "Bearer";
                });
            
            builder.Services.AddAuthorization(o => 
            {
                o.AddPolicy("Implicit", p =>
                {
                        p.RequireAuthenticatedUser();
                        p.RequireClaim("scope", "bookStoreApi");
                });
                o.AddPolicy("ClientCredentials", p =>
                {
                        // p.RequireAuthenticatedUser();
                        p.RequireClaim("scope", "bookStoreApiPostman");
                });
            });
            var app = builder.Build();
            
            // app.UseSerilogRequestLogging();
            // Configure the HTTP request pipeline.
            app.UseSwaggerUI(c =>
            {
                c.OAuthScopes("bookStoreApi", "bookStoreApiPostman");
            });
            app.UseSwagger();
            // app.UseHttpsRedirection();
            app.UseCors(IdentityServerCorsPolicy);
            app.UseAuthentication();
            app.UseRouting();
            
            app.UseAuthorization();
            app.Use(async (x, next) =>
            {
                var headers = x.Request.Headers.Select(x => $"{x.Key}={x.Value}").ToList();
                var headersCombined = string.Join("\n", headers);
                var readResult = await x.Request.BodyReader.ReadAsync();
                var cookies = x.Request.Cookies.Select(x => $"{x.Key}={x.Value}").ToList();
                var cookiesCombined = string.Join("\n", cookies);
                Console.WriteLine($"Req [{x.Request.Method}]: {x.Request.Path}\nHeaders:\n{headersCombined}\nCookies:\n{cookiesCombined}\nBody: \n{Encoding.UTF8.GetString(readResult.Buffer)}");
                await next();
            });
            app.UseEndpoints(ep => 
            {
                ep.MapControllers();
            });
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}