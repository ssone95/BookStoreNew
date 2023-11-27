using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Repository.Interfaces;

namespace BookStoreAPI.Data.Repository
{
	public class AuthorRepository : GenericRepository<Author>, IAuthorRepository
	{
		public AuthorRepository(BookStoreDbContext dbContext) : base(dbContext)
		{
		}
	}
}
