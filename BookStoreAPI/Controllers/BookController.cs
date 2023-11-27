using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;
using BookStoreAPI.Data.Repository;
using BookStoreAPI.Data.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreAPI.Controllers
{
	[ApiController]
	[Authorize("ClientCredentials")]
	[Route("[controller]")]
	public class BookController : ControllerBase
	{
		private readonly IBookService _bookService;
		public BookController(IBookService bookService) => _bookService = bookService;

		[HttpGet("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		public async Task<IActionResult> List()
		{
			var bookResponse = await _bookService.GetAllAsync();

			return bookResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(bookResponse),
				StatusCodes.Status400BadRequest => BadRequest(bookResponse),
				StatusCodes.Status404NotFound => NotFound(bookResponse),
				StatusCodes.Status500InternalServerError => BadRequest(bookResponse),
				_ => BadRequest(bookResponse)
			};
		}

		[HttpGet("[action]/{bookId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		public async Task<IActionResult> Get(int bookId)
		{
			var bookResponse = await _bookService.GetByIdAsync(bookId);

			return bookResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(bookResponse),
				StatusCodes.Status400BadRequest => BadRequest(bookResponse),
				StatusCodes.Status404NotFound => NotFound(bookResponse),
				StatusCodes.Status500InternalServerError => BadRequest(bookResponse),
				_ => BadRequest(bookResponse)
			};
		}

		[HttpGet("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[Consumes(contentType: "application/json")]
		public async Task<IActionResult> Create([FromBody] BookModel bookModel)
		{
			BaseResponse<BookModel> bookResponse = new ();
			if (ModelState.IsValid)
			{
				bookResponse = await _bookService.CreateAsync(bookModel);
			}
			else
			{
				var errors = from item in ModelState
							  where item.Value.Errors.Any()
							  select item.Value.Errors[0].ErrorMessage;
				bookResponse = new()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ErrorMessage = string.Join("\n", errors)
				};
			}

			return bookResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(bookResponse),
				StatusCodes.Status400BadRequest => BadRequest(bookResponse),
				StatusCodes.Status404NotFound => NotFound(bookResponse),
				StatusCodes.Status500InternalServerError => BadRequest(bookResponse),
				_ => BadRequest(bookResponse)
			};
		}

		[HttpPut("[action]/{bookId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[Consumes(contentType: "application/json")]
		public async Task<IActionResult> Update(int bookId, [FromBody] BookModel bookModel)
		{
			BaseResponse<BookModel> bookResponse = new();
			if (ModelState.IsValid)
			{
				bookResponse = await _bookService.UpdateAsync(bookId, bookModel);
			}
			else
			{
				var errors = from item in ModelState
							 where item.Value.Errors.Any()
							 select item.Value.Errors[0].ErrorMessage;
				bookResponse = new()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ErrorMessage = string.Join("\n", errors)
				};
			}

			return bookResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(bookResponse),
				StatusCodes.Status400BadRequest => BadRequest(bookResponse),
				StatusCodes.Status404NotFound => NotFound(bookResponse),
				StatusCodes.Status500InternalServerError => BadRequest(bookResponse),
				_ => BadRequest(bookResponse)
			};
		}

		[HttpDelete("[action]/{bookId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<BookModel>>))]
		public async Task<IActionResult> Delete(int bookId)
		{
			BaseResponse<BookModel> bookResponse = new();
			if (ModelState.IsValid)
			{
				bookResponse = await _bookService.DeleteAsync(bookId);
			}
			else
			{
				var errors = from item in ModelState
							 where item.Value.Errors.Any()
							 select item.Value.Errors[0].ErrorMessage;
				bookResponse = new()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ErrorMessage = string.Join("\n", errors)
				};
			}

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
