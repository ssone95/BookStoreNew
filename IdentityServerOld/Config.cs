using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "34dce41c-e91d-4953-9bf3-52f443369c8b",
                    Username = "testuser1",
                    Password = "Secret123$",
                    Claims = 
                    {
                        new Claim(JwtClaimTypes.Name, "Test User 1"),
                        new Claim(JwtClaimTypes.GivenName, "Test"),
                        new Claim(JwtClaimTypes.FamilyName, "User"),
                        new Claim(JwtClaimTypes.WebSite, "https://google.com")
                    }
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource 
                {
                    Scopes = new List<string>() { "bookStoreApiPostman", "bookStoreApi" },
                    ApiSecrets = new List<Secret>() 
                    {
                        new Secret("Secret123$".Sha256())
                        {
                            Type = "SharedSecret",
                            Description = "ApiResources Secret"
                        }
                    }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("bookStoreApi", "BoookStore API"),
                new ApiScope("bookStoreApiPostman", "BoookStore API Postman")
            };

        public static IEnumerable<Client> Clients =>
            new Client[] 
            { 
                new Client
                {
                    ClientId = "bookStoreClient",
                    ClientName = null,

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    // secret for authentication
                    ClientSecrets = new Secret[]
                    {
                        new Secret("Secret123$".Sha256())
                        {
                            Type = "SharedSecret",
                            Description = "bookStoreClient Secret"
                        }
                    },

                    RedirectUris = { "https://localhost:6001/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:6001/swagger/oauth2-redirect.html" },

                    // scopes that client has access to
                    AllowedScopes =
                    { 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "bookStoreApi",
                    }
                },
                
                new Client
                {
                    ClientId = "bookStoreClientPostman",
                    ClientName = null,

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    RedirectUris = new List<string>() { "https://oauth.pstmn.io/v1/callback", "https://oauth.pstmn.io/v1/browser-callback" },
                    PostLogoutRedirectUris = { "https://www.getpostman.com" },
                    AllowedCorsOrigins = { "http://localhost:6000", "https://localhost:6001" },
                    EnableLocalLogin = true,
                    
                    Claims = TestUsers[0].Claims.Select(x => new ClientClaim() { Type = x.Type, Value = x.Value, ValueType = x.ValueType }).ToList(),

                    // secret for authentication
                    ClientSecrets = new Secret[]
                    {
                        new Secret("Secret123$".Sha256())
                        {
                            Type = "SharedSecret",
                            Description = "bookStoreClientPostman Secret"
                        }
                    },

                    // scopes that client has access to
                    AllowedScopes = 
                    { 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "bookStoreApiPostman" 
                    }
                }
            };
    }
}