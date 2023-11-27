using BookStoreAPI.Data.Configuration;
using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Extensions
{
	public static class DbMigrationsHelper
	{
		public static async Task MigrateAndSeedDb(this IHost host, IConfiguration configuration)
		{
			using var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();

			await MigrateAndSeedBookStore(scope, configuration);

		}

		private static async Task MigrateAndSeedBookStore(IServiceScope scope, IConfiguration configuration)
		{
			var authors = configuration.GetSection("InitialDBCatalog:Authors").Get<List<AuthorConfiguration>>() ?? new List<AuthorConfiguration>();
			var books = configuration.GetSection("InitialDBCatalog:Books").Get<List<BookConfiguration>>() ?? new List<BookConfiguration>();

			using var bookStoreDbContext = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();

			await bookStoreDbContext.Database.MigrateAsync();

			await SeedAuthorsAndBooks(bookStoreDbContext, authors, books);
		}

		private static async Task SeedAuthorsAndBooks(BookStoreDbContext bookStoreDbContext, List<AuthorConfiguration> authors, List<BookConfiguration> books)
		{
			if (bookStoreDbContext.Authors.AsNoTracking().Count() < 1)
			{
				var authorsToSave = authors.Select(x => new Author()
				{
					AuthorId = x.Id,
					Name = x.Name
				});

				await bookStoreDbContext.Authors.AddRangeAsync(authorsToSave);
				await bookStoreDbContext.SaveChangesAsync();
			}

			if (bookStoreDbContext.Books.AsNoTracking().Count() < 1)
			{
				var dbAuthors = await bookStoreDbContext.Authors.AsNoTracking().ToListAsync();
				var booksToSave = books.Select(x => new Book()
				{
					BookId = x.Id,
					SubTitle = x.Subtitle,
					Title = x.Title,
					AuthorId = dbAuthors.FirstOrDefault(y => y.Name == x.Author).AuthorId
				});

				await bookStoreDbContext.Books.AddRangeAsync(booksToSave);
				await bookStoreDbContext.SaveChangesAsync();
			}
		}
	}
}
