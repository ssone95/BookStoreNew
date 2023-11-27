using System.Text.Json.Serialization;

namespace IdentityServerOld.Data.ConfigurationModels
{
	public class OAuthApiScope
	{
		[JsonPropertyName("Name")]
		public string Name { get; set; }
		[JsonPropertyName("DisplayName")]
		public string DisplayName { get; set; }
		[JsonPropertyName("Description")]
		public string Description { get; set; }
		[JsonPropertyName("Enabled")]
		public bool Enabled { get; set; }
		[JsonPropertyName("ShowInDiscoveryDocument")]
		public bool ShowInDiscoveryDocument { get; set; }
	}
}
