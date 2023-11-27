using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;
using BookStoreAPI.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Data.Repository
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        public BookRepository(BookStoreDbContext dbContext) : base(dbContext)
        {
        }

		public async Task<(int totalPages, List<Book> books)> SearchBooksAsync(BookSearchModel bookSearchModel)
		{
            int itemsPerPage = Math.Clamp(bookSearchModel.ItemsPerPage, 1, int.MaxValue);

            int currentPage = Math.Clamp(bookSearchModel.Page, 1, int.MaxValue);

            int itemsToSkip = Math.Clamp(itemsPerPage * (currentPage - 1), 0, int.MaxValue);

			string? normalizedTitle = bookSearchModel.Title?.ToLower();
            string? normalizedAuthor = bookSearchModel.Author?.ToLower();
			var booksQueryable = _dbContext.Books
                .Include(x => x.Author)
                .Where(x => string.IsNullOrEmpty(normalizedTitle) || x.Title.Contains(normalizedTitle))
                .Where(x => string.IsNullOrEmpty(normalizedAuthor) || x.Author.Name.Contains(normalizedAuthor))
                .AsNoTracking();

            var totalBooksCount = await booksQueryable.CountAsync();

			var totalBookPages = Math.Clamp(totalBooksCount / itemsPerPage, 1, Int32.MaxValue);

			var booksFromDb = await booksQueryable
                .Skip(itemsToSkip)
                .Take(itemsPerPage)
                .ToListAsync();

            return (totalPages: totalBookPages, books: booksFromDb);
		}
	}
}
