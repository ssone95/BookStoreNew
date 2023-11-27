using BookStoreAPI.Data.Models;
using BookStoreAPI.Data.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreAPI.Controllers
{
	[Authorize("Implicit")]
	[ApiController]
	[Route("Search")]
	public class SearchBooksController : ControllerBase
	{
		private readonly IBookService _bookService;

		public SearchBooksController(IBookService bookService) => _bookService = bookService;

		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[Consumes("application/json")]
		public async Task<IActionResult> List(BookSearchModel searchModel)
		{
			var bookResponse = await _bookService.SearchBooksAsync(searchModel);

			return bookResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(bookResponse),
				StatusCodes.Status400BadRequest => BadRequest(bookResponse),
				StatusCodes.Status404NotFound => NotFound(bookResponse),
				StatusCodes.Status500InternalServerError => BadRequest(bookResponse),
				_ => BadRequest(bookResponse)
			};
		}
	}
}
