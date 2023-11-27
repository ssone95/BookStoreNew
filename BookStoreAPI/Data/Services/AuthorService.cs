using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Mappers;
using BookStoreAPI.Data.Models;
using BookStoreAPI.Data.Repository.Interfaces;
using BookStoreAPI.Data.Services.Interfaces;

namespace BookStoreAPI.Data.Services
{
	public class AuthorService : IAuthorService
	{
		private readonly IAuthorRepository _repository;

		public AuthorService(IAuthorRepository authorRepository) => _repository = authorRepository;

		public async Task<BaseResponse<AuthorModel?>> GetByIdAsync(int id)
		{
			BaseResponse<AuthorModel?> response = new ();
			try
			{
				var author = await _repository.GetByIdAsync(id);
				var authorModel = author?.Map();
				response.StatusCode = authorModel != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
				response.Result = authorModel;
			}
			catch (Exception ex)
			{
				response.StatusCode = StatusCodes.Status500InternalServerError;
				response.ErrorMessage = ex.Message;
			}
			return response;
		}

		public async Task<BaseResponse<IEnumerable<AuthorModel>>> GetAllAsync()
		{
			BaseResponse<IEnumerable<AuthorModel>> response = new();
			try
			{
				var authors = await _repository.GetAllAsync();
				var authorModels = authors?.Map() ?? new List<AuthorModel>();
				response.Result = authorModels;
				response.StatusCode = StatusCodes.Status200OK;
			}
			catch (Exception ex)
			{
				response.StatusCode = StatusCodes.Status500InternalServerError;
				response.ErrorMessage = ex.Message;
			}
			return response;
		}

		public async Task<BaseResponse<AuthorModel>> CreateAsync(AuthorModel authorModel)
		{
			BaseResponse<AuthorModel> response = new ();
			try
			{
				var author = authorModel.Map();
				var createdAuthor = await _repository.CreateAsync(author);
				await _repository.SaveChangesAsync();

				response.Result = createdAuthor.Map();
				response.StatusCode = createdAuthor != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
			}
			catch (Exception ex)
			{
				response.ErrorMessage = ex.Message;
				response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			return response;
		}

		public async Task<BaseResponse<AuthorModel>> UpdateAsync(int id, AuthorModel authorModel)
		{
			BaseResponse<AuthorModel> response = new();
			try
			{
				var existingAuthorModel = await GetByIdAsync(id);
				if (existingAuthorModel.Result != null)
				{
					response.StatusCode = StatusCodes.Status404NotFound;
					return response;
				}

				var author = authorModel!.Map();
				author.AuthorId = id;

				var updatedAuthor = await _repository.UpdateAsync(author);
				await _repository.SaveChangesAsync();

				response.Result = updatedAuthor.Map();
				response.StatusCode = updatedAuthor != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
			}
			catch (Exception ex)
			{
				response.ErrorMessage = ex.Message;
				response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			return response;
		}

		public async Task<BaseResponse<AuthorModel>> DeleteAsync(int id)
		{
			BaseResponse<AuthorModel> response = new();
			try
			{
				var existingAuthorModel = await GetByIdAsync(id);
				if (existingAuthorModel.Result != null)
				{
					response.StatusCode = StatusCodes.Status404NotFound;
					return response;
				}

				var existingAuthor = existingAuthorModel.Result!.Map();
				var deletedAuthor = await _repository.DeleteAsync(existingAuthor);

				await _repository.SaveChangesAsync();

				response.Result = deletedAuthor.Map();
				response.StatusCode = deletedAuthor != null ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;
			}
			catch (Exception ex)
			{
				response.ErrorMessage = ex.Message;
				response.StatusCode = StatusCodes.Status500InternalServerError;
			}
			return response;
		}
	}
}
