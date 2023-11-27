using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;

namespace BookStoreAPI.Data.Repository.Interfaces
{
	public interface IBookRepository : IGenericRepository<Book>
	{
		Task<(int totalPages, List<Book> books)> SearchBooksAsync(BookSearchModel bookSearchModel);
	}
}
