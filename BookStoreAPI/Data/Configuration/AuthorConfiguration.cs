using System.Text.Json.Serialization;

namespace BookStoreAPI.Data.Configuration
{
	public class AuthorConfiguration
	{
		[JsonPropertyName("Id")]
		public int Id { get; set; }
		[JsonPropertyName("Name")]
		public string Name { get; set; }
	}
}
