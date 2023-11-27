using System.Text.Json.Serialization;

namespace BookStoreAPI.Data.Configuration
{
	public class BookConfiguration
	{
		[JsonPropertyName("Id")]
		public int Id { get; set; }
		[JsonPropertyName("Title")]
		public string Title { get; set; }
		[JsonPropertyName("Subtitle")]
		public string Subtitle { get; set; }
		[JsonPropertyName("Author")]
		public string Author { get; set; }
	}
}
