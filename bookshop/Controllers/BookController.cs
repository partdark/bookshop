using Application.Dto;
using Application.Interfaces;
using Infrastructure.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("books")]
        public async Task<ActionResult<List<Guid>>> GetBooksIds()
            => await _bookService.GetBookSIds();

        [HttpGet("catalog/{id:guid}")]
        public async Task<ActionResult<BookResponseDto>> GetBookById([FromRoute] Guid id)
        {
            var book = await _bookService.GetById(id);
            return book == null ? NotFound() : Ok(book);
        }

        [HttpGet("catalog")]
        public async Task<ActionResult<ListWithBooksBaseData>> Catalog(
            int pageCapacity = 20, int pageNumber = 1,
            string orderBy = "Title", bool desc = false, string? titleContains = null, bool countMoreThenZero = true)
            => await _bookService.BookShowcase(pageCapacity, pageNumber, orderBy, desc, titleContains, countMoreThenZero);

        [Authorize(Roles = "Admin")]
        [HttpPost("book/add")]
        public async Task<ActionResult<Guid>> CreateBook([FromBody] AddBookDto bookDto)
        {
            var id = await _bookService.AddBook(bookDto);
            return id != null ? Ok(id) : BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("book/createbookwithfullinfo")]
        public async Task<ActionResult<Guid>> CreateBookWithAuthorsAndGenres([FromBody] AddBookWithAuthorsAndGenresDto dto)
            => await _bookService.CreateBookWithIndicatingExistingAuthorsAndgenres(dto.BookDto, dto.AuthorsIds, dto.GenresIds);

        [Authorize(Roles = "Admin")]
        [HttpDelete("book/delete/{id:guid}")]
        public async Task<IActionResult> DeleteBook([FromRoute] Guid id)
            => await _bookService.DeleteAsync(id) ? Ok() : NotFound();

        [Authorize(Roles = "Admin")]
        [HttpPut("book/update/{id:guid}")]
        public async Task<ActionResult<BookResponseDto>> UpdateBook([FromRoute] Guid id, [FromBody] BookResponseDto book)
        {
            if (id != book.Id) return BadRequest("Id не совпадают");
            var result = await _bookService.UpdateBook(book);
            return result == null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("book/patch/{id:guid}")]
        public async Task<ActionResult<AddBookDto>> PatchBook([FromRoute] Guid id, [FromBody] JsonPatchDocument<AddBookDto> patch)
        {
            var result = await _bookService.PatchBook(id, patch);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
