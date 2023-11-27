using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using BookStoreAPI.AuthorizationFilters;
using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Repository;
using BookStoreAPI.Data.Repository.Interfaces;
using BookStoreAPI.Data.Services;
using BookStoreAPI.Data.Services.Interfaces;
using BookStoreAPI.Extensions;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

internal class Program
{
    private const string IdentityServerCorsPolicy = "IdentityServerCorsPolicy";
	private static readonly string MigrationsAssemblyName = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

	private static async Task Main(string[] args)
    {
        try
		{
			Log.Information("Starting host...");
			var builder = WebApplication.CreateBuilder(args);
			ConfigureServices(builder);

			var app = builder.Build();
			ConfigureApp(app); 
			await app.RunAsync();
		}
		catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly: {ex}.", ex);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

	private static async void ConfigureApp(WebApplication app)
	{
		// Configure the HTTP request pipeline.
		app.UseSwaggerUI();
		app.UseSwagger();
		app.UseCors(IdentityServerCorsPolicy);
		app.UseAuthentication();
		app.UseRouting();

		app.UseAuthorization();
#pragma warning disable ASP0014 // Suggest using top level route registrations
		app.UseEndpoints(ep =>
		{
			ep.MapControllers();
		});
#pragma warning restore ASP0014 // Suggest using top level route registrations

		await app.MigrateAndSeedDb(app.Configuration);
	}

	private static void ConfigureServices(WebApplicationBuilder builder)
	{
		builder.Services.AddLogging(options =>
		{
			options.ClearProviders();
			options.AddSerilog();
		});

		builder.Services.AddHttpLogging(c =>
		{
			if (builder.Environment.IsDevelopment())
			{
				c.LoggingFields = HttpLoggingFields.All;
			}
			else
			{
				c.LoggingFields = HttpLoggingFields.RequestHeaders;
			}
		});

		builder.Services.AddCors(c =>
		{
			c.AddPolicy(name: IdentityServerCorsPolicy, b =>
			{
				var corsAllowedOrigins = builder.Configuration.GetSection("Security:CorsAllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
				b.WithOrigins(corsAllowedOrigins)
					.AllowCredentials().AllowAnyHeader().AllowAnyMethod();
			});
		});
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			string baseOpenIdUrl = builder.Configuration.GetValue<string>("Security:IdentityServer:OpenIdBaseUrl") ?? string.Empty;
			string authorizationUrl = builder.Configuration.GetValue<string>("Security:IdentityServer:ImplicitAuthorizationUrl") ?? string.Empty;
			string tokenUrl = builder.Configuration.GetValue<string>("Security:IdentityServer:ClientCredentialsTokenUrl") ?? string.Empty;
			string implicitFlowScope = builder.Configuration.GetValue<string>("Security:IdentityServer:ImplicitFlowScope") ?? string.Empty;
			string clientCredentialsFlowScope = builder.Configuration.GetValue<string>("Security:IdentityServer:ClientCredentialsFlowScope") ?? string.Empty;
			c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
			{
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.OAuth2,
				Flows = new OpenApiOAuthFlows()
				{
					Implicit = new OpenApiOAuthFlow()
					{
						AuthorizationUrl = new Uri(baseOpenIdUrl + authorizationUrl),
						Scopes = { { "bookStoreApi.read", "Book Store Implicit Flow" } }
					},
					ClientCredentials = new OpenApiOAuthFlow()
					{
						TokenUrl = new Uri(baseOpenIdUrl + tokenUrl),
						Scopes = { { "bookStoreApi.write", "Book Store ClientCredentials Flow" } },
					}
				},
				OpenIdConnectUrl = new Uri(baseOpenIdUrl),
				Scheme = "Bearer",
				Name = "oauth2",
			});

			c.OperationFilter<SwaggerAuthorizationFilter>();
		});
		builder.Services.AddControllers();
		builder.Services.AddAuthentication("Bearer")
			.AddJwtBearer("Bearer", o =>
			{
				string authority = builder.Configuration.GetValue<string>("Security:IdentityServer:Authority") ?? string.Empty;
				o.Authority = authority;
				var isDevEnvironment = builder.Environment.IsDevelopment() 
					|| builder.Environment.IsEnvironment("Docker");
				o.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateAudience = !isDevEnvironment,
					ValidateIssuer = !isDevEnvironment,
					ValidateIssuerSigningKey = !isDevEnvironment,
					RequireExpirationTime = true,
					ValidateLifetime = true
				};
				if (isDevEnvironment)
				{
					IdentityModelEventSource.ShowPII = true;
					o.TokenValidationParameters.SignatureValidator = delegate (string token, TokenValidationParameters tokenValidationParameters)
					{
						var jwt = new JwtSecurityToken(token);

						return jwt;
					};
				}
				// Disabling for development purposes

				o.MetadataAddress = builder.Configuration.GetValue<string>("Security:IdentityServer:MetadataAddress") ?? string.Empty;
				o.RequireHttpsMetadata = !isDevEnvironment;
			});


		builder.Services.AddDbContext<BookStoreDbContext>(options =>
		{
			options.EnableDetailedErrors(true);
			options.EnableSensitiveDataLogging(true);
			var connString = builder.Configuration.GetConnectionString("BookStoreDb") ?? string.Empty;
			options.UseSqlServer(connString, sql =>
			{
				sql.EnableRetryOnFailure();
				sql.MigrationsAssembly(MigrationsAssemblyName);
			});
			//options.UseSqlite(connString, sql => sql.MigrationsAssembly(MigrationsAssemblyName));
		});

		builder.Services.AddAuthorization(o =>
		{
			o.AddPolicy("Implicit", p =>
			{
				p.RequireAuthenticatedUser();
				p.RequireClaim("scope", "bookStoreApi.read");
			});
			o.AddPolicy("ClientCredentials", p =>
			{
				p.RequireAuthenticatedUser();
				p.RequireClaim("scope", "bookStoreApi.write");
			});
		});


		#region Register DI services
		builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
		builder.Services.AddScoped<IBookRepository, BookRepository>();

		builder.Services.AddScoped<IAuthorService, AuthorService>();
		builder.Services.AddScoped<IBookService, BookService>();
		#endregion


		builder.Host.UseSerilog((context, configuration) =>
		{
			configuration.ReadFrom.Configuration(context.Configuration);
		});
	}
}