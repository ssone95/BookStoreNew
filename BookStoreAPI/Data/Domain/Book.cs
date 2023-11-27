using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStoreAPI.Data.Domain
{
	public class Book
	{
		[Required]
		[Column("bookId")]
		public int BookId { get; set; }

		[Required]
		[Column("authorId")]
		public int AuthorId { get; set; }
		public Author Author { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Title is required, and must be between 3 and 100 characters long!")]
		[MinLength(3, ErrorMessage = "Title must be at least 3 characters long!")]
		[MaxLength(100, ErrorMessage = "Title must not be longer than 100 characters!")]
		[Column("title")]
		public string Title { get; set; }

		[Column("subTitle")]
		public string SubTitle { get; set; }
		public Book() { }
		public Book(int authorId, string title, string subTitle)
		{
			AuthorId = authorId;
			Title = title;
			SubTitle = subTitle;
		}
	}
}
