using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Data.Models
{
	public class AuthorModel
	{
		[JsonPropertyName("authorId")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Author Id is required!")]
		public int Id { get; set; }

		[JsonPropertyName("name")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required!")]
		[MinLength(3, ErrorMessage = "Name must be at least 3 characters long!")]
		[MaxLength(100, ErrorMessage = "Name must be at maximum 100 characters long!")]
		public string Name { get; set; }
	}
}
