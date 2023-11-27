using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAPI.Data.Domain
{
	public class Author
	{
		[Required(AllowEmptyStrings = false, ErrorMessage = "authorId is required, and must be between 3 and 100 characters long!")]
		[Column("authorId")]
		[Key]
		public int AuthorId { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Name is required, and must be between 3 and 100 characters long!")]
		[MinLength(3, ErrorMessage = "Name must be at least 3 characters long!")]
		[MaxLength(100, ErrorMessage = "Name must not be longer than 100 characters!")]
		[Column("name")]
		public string Name { get; set; }

	}
}
