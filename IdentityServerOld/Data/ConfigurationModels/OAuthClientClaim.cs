using System.Text.Json.Serialization;

namespace IdentityServerOld.Data.ConfigurationModels
{
	public class OAuthClientClaim
	{
		[JsonPropertyName("Value")]
		public string Value { get; set; }
		[JsonPropertyName("Type")]
		public string Type { get; set; }
	}
}
