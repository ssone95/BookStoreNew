using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Data.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		protected readonly BookStoreDbContext _dbContext;

		public GenericRepository(BookStoreDbContext dbContext) => _dbContext = dbContext;

		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbContext.Set<T>().FindAsync(id);
		}
		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbContext.Set<T>().ToListAsync();
		}

		public async Task<T> CreateAsync(T entity)
		{
			await _dbContext.Set<T>().AddAsync(entity);
			return await Task.FromResult(entity);
		}

		public async Task<T> UpdateAsync(T entity)
		{
			_dbContext.Set<T>().Update(entity);
			return await Task.FromResult(entity);
		}

		public async Task<T> DeleteAsync(T entity)
		{
			_dbContext.Set<T>().Remove(entity);
			return await Task.FromResult(entity);
		}


		public async Task SaveChangesAsync() 
			=> await _dbContext.SaveChangesAsync();
	}
}
