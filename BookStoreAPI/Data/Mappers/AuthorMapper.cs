using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;

namespace BookStoreAPI.Data.Mappers
{
	public static class AuthorMapper
	{
		public static AuthorModel Map(this Author author) => new ()
		{
			Id = author.AuthorId,
			Name = author.Name,
		};

		public static Author Map(this AuthorModel authorModel) => new ()
		{
			AuthorId = authorModel.Id,
			Name = authorModel.Name
		};

		public static IEnumerable<AuthorModel> Map(this IEnumerable<Author> authors) 
			=> authors.Select(x => x.Map());

		public static IEnumerable<Author> Map(this IEnumerable<AuthorModel> authorModels) 
			=> authorModels.Select(x => x.Map());
	}
}
