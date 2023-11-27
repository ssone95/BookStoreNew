using BookStoreAPI.Data.Contexts;
using BookStoreAPI.Data.Domain;
using BookStoreAPI.Data.Models;
using BookStoreAPI.Data.Services;
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
	public class AuthorController : ControllerBase
	{
		private readonly IAuthorService _authorService;

		public AuthorController(IAuthorService authorService) => _authorService = authorService;

		[HttpGet("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		public async Task<IActionResult> List()
		{
			var authorResponse = await _authorService.GetAllAsync();

			return authorResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(authorResponse),
				StatusCodes.Status400BadRequest => BadRequest(authorResponse),
				StatusCodes.Status404NotFound => NotFound(authorResponse),
				StatusCodes.Status500InternalServerError => BadRequest(authorResponse),
				_ => BadRequest(authorResponse)
			};
		}

		[HttpGet("[action]/{authorId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		public async Task<IActionResult> Get(int authorId)
		{
			var authorResponse = await _authorService.GetByIdAsync(authorId);

			return authorResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(authorResponse),
				StatusCodes.Status400BadRequest => BadRequest(authorResponse),
				StatusCodes.Status404NotFound => NotFound(authorResponse),
				StatusCodes.Status500InternalServerError => BadRequest(authorResponse),
				_ => BadRequest(authorResponse)
			};
		}

		[HttpPost("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[Consumes(contentType: "application/json")]
		public async Task<IActionResult> Create([FromBody] AuthorModel authorModel)
		{
			BaseResponse<AuthorModel> authorResponse = new();
			if (ModelState.IsValid)
			{
				authorResponse = await _authorService.CreateAsync(authorModel);
			}
			else
			{
				var errors = from item in ModelState
							 where item.Value.Errors.Any()
							 select item.Value.Errors[0].ErrorMessage;
				authorResponse = new()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ErrorMessage = string.Join("\n", errors)
				};
			}

			return authorResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(authorResponse),
				StatusCodes.Status400BadRequest => BadRequest(authorResponse),
				StatusCodes.Status404NotFound => NotFound(authorResponse),
				StatusCodes.Status500InternalServerError => BadRequest(authorResponse),
				_ => BadRequest(authorResponse)
			};
		}

		[HttpPut("[action]/{authorId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[Consumes(contentType: "application/json")]
		public async Task<IActionResult> Update(int authorId, [FromBody] AuthorModel authorModel)
		{
			BaseResponse<AuthorModel> authorResponse = new();
			if (ModelState.IsValid)
			{
				authorResponse = await _authorService.UpdateAsync(authorId, authorModel);
			}
			else
			{
				var errors = from item in ModelState
							 where item.Value.Errors.Any()
							 select item.Value.Errors[0].ErrorMessage;
				authorResponse = new()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ErrorMessage = string.Join("\n", errors)
				};
			}

			return authorResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(authorResponse),
				StatusCodes.Status400BadRequest => BadRequest(authorResponse),
				StatusCodes.Status404NotFound => NotFound(authorResponse),
				StatusCodes.Status500InternalServerError => BadRequest(authorResponse),
				_ => BadRequest(authorResponse)
			};
		}

		[HttpDelete("[action]/{authorId}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(BaseResponse<IEnumerable<AuthorModel>>))]
		public async Task<IActionResult> Delete(int authorId)
		{
			BaseResponse<AuthorModel> authorResponse = new();
			if (ModelState.IsValid)
			{
				authorResponse = await _authorService.DeleteAsync(authorId);
			}
			else
			{
				var errors = from item in ModelState
							 where item.Value.Errors.Any()
							 select item.Value.Errors[0].ErrorMessage;
				authorResponse = new()
				{
					StatusCode = StatusCodes.Status400BadRequest,
					ErrorMessage = string.Join("\n", errors)
				};
			}

			return authorResponse.StatusCode switch
			{
				StatusCodes.Status200OK => Ok(authorResponse),
				StatusCodes.Status400BadRequest => BadRequest(authorResponse),
				StatusCodes.Status404NotFound => NotFound(authorResponse),
				StatusCodes.Status500InternalServerError => BadRequest(authorResponse),
				_ => BadRequest(authorResponse)
			};
		}
	}
}