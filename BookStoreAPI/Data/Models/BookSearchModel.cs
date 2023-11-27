using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
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
		[Range(1, int.MaxValue - 1, ConvertValueInInvariantCulture = true, ErrorMessage = "Page must be set to at least 1!")]
		public int Page { get; set; } = 1;
		[JsonPropertyName("itemsPerPage")]
		[Range(1, int.MaxValue - 1, ConvertValueInInvariantCulture = true, ErrorMessage = "Items per page must be set to at least 1!")]
		public int ItemsPerPage { get; set; } = 3;
	}
}
