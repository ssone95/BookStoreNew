using System.Text.Json.Serialization;

namespace IdentityServerOld.Data.ConfigurationModels
{
	public class OAuthClientsecret
	{
		[JsonPropertyName("Type")]
		public string Type { get; set; }
		[JsonPropertyName("Description")]
		public string Description { get; set; }
		[JsonPropertyName("Value")]
		public string Value { get; set; }
	}
}
