using Application.Dto;
using Application.Interfaces;
using Infrastructure.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> test()
        {
            return Ok();
        }

        [HttpGet("GenerateGuid")]
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        [HttpGet("book/allIds")]
        public async Task<ActionResult<List<Guid>>> GetBooksIds()
        {
            return await _bookService.GetBookSIds();
        }

        [HttpGet("/catalog/{id:guid}")]
        public async Task<ActionResult<BookResponseDto>> GetBookById([FromRoute] Guid id)
        {
            var bookResponse = await _bookService.GetById(id);
            if (bookResponse == null)
            {
                return NotFound();
            }
            return Ok(bookResponse);
        }
        [HttpPost("createbook")]
        public async Task<ActionResult<Guid>> CeateBook([FromBody] AddBookDto bookDto)
        {
            var bookId = await _bookService.AddBook(bookDto);

            if (bookId != null)
            {
                return Ok(bookId);
            }
            return BadRequest();
        }

        [HttpGet("/catalog")]
        public async Task<ActionResult<ListWithBooksBaseData>> Catalog(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool desc = false, string? titleContains = null)
        {
            return await _bookService.BookShowcase(pageCapacity, pageNumber, orderBy, desc, titleContains);
        }

        [HttpPost("/createbookwithfullinfo")]
        public async Task<ActionResult<Guid>> CreateBookWithIndicatingExistingAuthorsAndgenres([FromBody] AddBookWithAuthorsAndGenresDto bookInfoDto)         
        {
            var result = await _bookService.CreateBookWithIndicatingExistingAuthorsAndgenres(bookInfoDto.BookDto, bookInfoDto.AuthorsIds, bookInfoDto.GenresIds);
            return result;
        }

        [HttpDelete("/delete/{bookId:guid}")]
        public async Task<ActionResult> DeleteBook([FromRoute] Guid bookId) { 
        var result = await _bookService.DeleteAsync(bookId);
            if (result)
            {
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPut("/update/{id:guid}")]
        public async Task<ActionResult<BookResponseDto>> UpdateBook([FromRoute] Guid id,[FromBody] BookResponseDto book)
        {
            if (id != book.Id )
            {
                return BadRequest("Id не совпадают"); 
            }

          var result =   await _bookService.UpdateBook(book);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        [HttpPatch("/patch/{id:guid}")]
        public async Task<ActionResult<AddBookDto>> PatchBook([FromRoute] Guid id,[FromBody] JsonPatchDocument<AddBookDto> book) 
        { 
            var patchedBook = await _bookService.PatchBook(id ,book);
            if (patchedBook == null)
            {
                return NotFound();
            }
            return Ok(patchedBook);
        
        }
        
    }
}
