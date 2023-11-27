using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Mappers;
using BookStoreAPI.Data.Models;
using BookStoreAPI.Data.Repository.Interfaces;
using BookStoreAPI.Data.Services.Interfaces;

namespace BookStoreAPI.Data.Services
{
	public class BookService : IBookService
	{
		private readonly IBookRepository _repository;
		public BookService(IBookRepository repository) => _repository = repository;

		public async Task<BaseResponse<BookModel?>> GetByIdAsync(int id)
		{
			BaseResponse<BookModel?> response = new();
			try
			{
				var book = await _repository.GetByIdAsync(id);
				var bookModel = book?.Map();
				response.StatusCode = bookModel != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
				response.Result = bookModel;
			}
			catch (Exception ex)
			{
				response.StatusCode = StatusCodes.Status500InternalServerError;
				response.ErrorMessage = ex.Message;
			}
			return response;
		}

		public async Task<BaseResponse<IEnumerable<BookModel>>> GetAllAsync()
		{
			BaseResponse<IEnumerable<BookModel>> response = new();
			try
			{
				var books = await _repository.GetAllAsync();
				var bookModels = books?.Map() ?? new List<BookModel>();
				response.Result = bookModels;
				response.StatusCode = StatusCodes.Status200OK;
			}
			catch (Exception ex)
			{
				response.StatusCode = StatusCodes.Status500InternalServerError;
				response.ErrorMessage = ex.Message;
			}
			return response;
		}

		public async Task<BaseResponse<BookModel>> CreateAsync(BookModel bookModel)
		{
			BaseResponse<BookModel> response = new();
			try
			{
				var book = bookModel.Map();
				var createdBook = await _repository.CreateAsync(book);
				await _repository.SaveChangesAsync();

				response.Result = createdBook.Map();
				response.StatusCode = createdBook != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
			}
			catch (Exception ex)
			{
				response.ErrorMessage = ex.Message;
				response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			return response;
		}

		public async Task<BaseResponse<BookModel>> UpdateAsync(int id, BookModel bookModel)
		{
			BaseResponse<BookModel> response = new();
			try
			{
				var existingBookModel = await GetByIdAsync(id);
				if (existingBookModel.Result == null)
				{
					response.StatusCode = StatusCodes.Status404NotFound;
					return response;
				}

				var book = bookModel!.Map();
				book.BookId = id;

				var updatedBook = await _repository.UpdateAsync(book);
				await _repository.SaveChangesAsync();

				response.Result = updatedBook.Map();
				response.StatusCode = updatedBook != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
			}
			catch (Exception ex)
			{
				response.ErrorMessage = ex.Message;
				response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			return response;
		}

		public async Task<BaseResponse<BookModel>> DeleteAsync(int id)
		{
			BaseResponse<BookModel> response = new();
			try
			{
				var existingBookModel = await GetByIdAsync(id);
				if (existingBookModel.Result == null)
				{
					response.StatusCode = StatusCodes.Status404NotFound;
					return response;
				}

				var existingBook = existingBookModel.Result!.Map();
				var deletedBook = await _repository.DeleteAsync(existingBook);

				await _repository.SaveChangesAsync();

				response.Result = deletedBook.Map();
				response.StatusCode = deletedBook != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
			}
			catch (Exception ex)
			{
				response.ErrorMessage = ex.Message;
				response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			return response;
		}

		public async Task<BaseResponse<IEnumerable<BookModel>>> SearchBooksAsync(BookSearchModel bookSearchModel)
		{
			BaseResponse<IEnumerable<BookModel>> response = new();
			try
			{
				var searchResult = await _repository.SearchBooksAsync(bookSearchModel);
				var bookModels = searchResult.books?.Map() ?? new List<BookModel>();
				var totalPages = searchResult.totalPages;

				response.TotalPages = totalPages;
				response.Result = bookModels;
				response.StatusCode = StatusCodes.Status200OK;
				response.Page = bookSearchModel.Page;
			}
			catch (Exception ex)
			{
				response.StatusCode = StatusCodes.Status500InternalServerError;
				response.ErrorMessage = ex.Message;
				response.TotalPages = 0;
				response.Page = 0;
			}
			return response;
		}
	}
}
