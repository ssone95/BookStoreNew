using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IdentityServerOld.Data.ConfigurationModels
{
	public class AppUserConfig
	{
		[JsonPropertyName("Email")]
		public string UserName { get; set; }
		[JsonPropertyName("Password")]
		public string Password { get; set; }
		[JsonPropertyName("Email")]
		public string NormalizedUserName { get; set; }
		[JsonPropertyName("Email")]
		public string Email { get; set; }
		[JsonPropertyName("NormalizedEmail")]
		public string NormalizedEmail { get; set; }
		[JsonPropertyName("EmailConfigured")]
		public bool EmailConfigured { get; set; }
		[JsonPropertyName("PhoneNumberConfirmed")]
		public bool PhoneNumberConfirmed { get; set; }

		[JsonPropertyName("Roles")]
		public List<string> Roles { get; set; }
	}
}
