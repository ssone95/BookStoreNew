using BookStoreAPI.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Data.Contexts
{
	public class BookStoreDbContext : DbContext
	{
		public DbSet<Author> Authors { get; set; }
		public DbSet<Book> Books { get; set; }
		public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options)
			: base(options)
		{
		}
	}
}
