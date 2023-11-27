using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;

namespace BookStoreAPI.Data.Services.Interfaces
{
	public interface IBookService
	{
		Task<BaseResponse<BookModel>> GetByIdAsync(int id);
		Task<BaseResponse<IEnumerable<BookModel>>> GetAllAsync();
		Task<BaseResponse<IEnumerable<BookModel>>> SearchBooksAsync(BookSearchModel bookSearchModel);
		Task<BaseResponse<BookModel>> CreateAsync(BookModel bookModel);
		Task<BaseResponse<BookModel>> UpdateAsync(int id, BookModel bookModel);
		Task<BaseResponse<BookModel>> DeleteAsync(int id);
	}
}
