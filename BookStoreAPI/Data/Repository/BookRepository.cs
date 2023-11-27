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
            int itemsPerPage = bookSearchModel.ItemsPerPage;
            if (itemsPerPage < 1)
                itemsPerPage = 1;

            int currentPage = bookSearchModel.Page;
            if (currentPage < 1)
                currentPage = 1;

            int itemsToSkip = itemsPerPage * (bookSearchModel.Page - 1);

            var booksQueryable = _dbContext.Books
                .Include(x => x.Author)
                .Where(x => string.IsNullOrEmpty(bookSearchModel.Title) || x.Title.ToLower().Contains(bookSearchModel.Title.ToLower()))
                .Where(x => string.IsNullOrEmpty(bookSearchModel.Author) || x.Author.Name.ToLower().Contains(bookSearchModel.Author.ToLower()));

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
