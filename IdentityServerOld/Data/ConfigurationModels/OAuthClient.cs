using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IdentityServerOld.Data.ConfigurationModels
{
	public class OAuthClient
	{
		[JsonPropertyName("ClientId")]
		public string ClientId { get; set; }
		[JsonPropertyName("ClientName")]
		public string ClientName { get; set; }
		[JsonPropertyName("Enabled")]
		public bool Enabled { get; set; }
		[JsonPropertyName("AllowedGrantTypes")]
		public List<string> AllowedGrantTypes { get; set; }
		[JsonPropertyName("AllowedScopes")]
		public List<string> AllowedScopes { get; set; }
		[JsonPropertyName("AllowAccessTokensViaBrowser")]
		public bool AllowAccessTokensViaBrowser { get; set; }
		[JsonPropertyName("RequireConsent")]
		public bool RequireConsent { get; set; }
		[JsonPropertyName("RedirectUris")]
		public List<string> RedirectUris { get; set; }
		[JsonPropertyName("PostLogoutRedirectUris")]
		public List<string> PostLogoutRedirectUris { get; set; }
		[JsonPropertyName("AllowedCorsOrigins")]
		public List<string> AllowedCorsOrigins { get; set; }
		[JsonPropertyName("EnableLocalLogin")]
		public bool EnableLocalLogin { get; set; }
		[JsonPropertyName("ClientSecrets")]
		public List<OAuthClientsecret> ClientSecrets { get; set; }
		[JsonPropertyName("Claims")]
		public List<OAuthClientClaim> Claims { get; set; }

	}
}
