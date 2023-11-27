using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookStoreAPI.Data.Models
{
	public class BookModel
	{
		[JsonPropertyName("bookId")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Book Id is required!")]
		public int Id { get; set; }
		[JsonPropertyName("authorId")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "Author Id is required!")]
		public int AuthorId { get; set; }
		[JsonPropertyName("title")]
		[MinLength(3, ErrorMessage = "Title must be at least 3 characters long!")]
		[MaxLength(100, ErrorMessage = "Title must be at maximum 100 characters long!")]
		[Required(AllowEmptyStrings = false, ErrorMessage = "title is required!")]
		public string Title { get; set; }
		[JsonPropertyName("subTitle")]
		public string SubTitle { get; set; }

		[JsonPropertyName("author")]
		public AuthorModel Author { get; set; }
	}
}
