using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Data.Models
{
	public class BookSearchModel
	{
		[JsonPropertyName("title")]
		[ValidateNever]
		public string Title { get; set; }
		[JsonPropertyName("author")]
		[ValidateNever]
		public string Author { get; set; }

		[JsonPropertyName("page")]
		public int Page { get; set; } = 1;
		[JsonPropertyName("itemsPerPage")]
		public int ItemsPerPage { get; set; } = 3;
	}
}
