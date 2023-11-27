using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;

namespace BookStoreAPI.Data.Mappers
{
	public static class BookMapper
	{
		public static BookModel Map(this Book book) => new()
		{
			AuthorId = book.AuthorId,
			Id = book.BookId,
			SubTitle = book.SubTitle,
			Title = book.Title,
			Author = book.Author?.Map()
		};

		public static Book Map(this BookModel bookModel) => new()
		{
			Title = bookModel.Title,
			SubTitle = bookModel.SubTitle,
			AuthorId = bookModel.AuthorId,
			BookId = bookModel.Id
		};

		public static IEnumerable<BookModel> Map(this IEnumerable<Book> books)
			=> books.Select(x => x.Map());

		public static IEnumerable<Book> Map(this IEnumerable<BookModel> bookModels)
			=> bookModels.Select(x => x.Map());
	}
}
