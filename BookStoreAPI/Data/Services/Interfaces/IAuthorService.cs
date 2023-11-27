using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;

namespace BookStoreAPI.Data.Services.Interfaces
{
	public interface IAuthorService
	{
		Task<BaseResponse<AuthorModel>> GetByIdAsync(int id);
		Task<BaseResponse<IEnumerable<AuthorModel>>> GetAllAsync();
		Task<BaseResponse<AuthorModel>> CreateAsync(AuthorModel authorModel);
		Task<BaseResponse<AuthorModel>> UpdateAsync(int id, AuthorModel authorModel);
		Task<BaseResponse<AuthorModel>> DeleteAsync(int id);
	}
}
