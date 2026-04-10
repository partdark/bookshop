using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }


        [HttpGet("/author/{id:guid}")]
        public async Task<ActionResult<AuthorInfoDto>> GetByIdAsync(Guid id)
        {
            var result = await _authorService.GetByIdAsync(id);
            if (result == null) { return NotFound(); }
            return Ok(result);
        }

        [HttpGet("/authors")]
        public async Task<ActionResult<List<Guid>>> GetAuthorsIds()
        {
            var result = await _authorService.GetIdsAsync();
            return Ok(result);
        }

        [HttpPut("/author/add")]

        public async Task<ActionResult<AuthorResponseDto>> AddAuthorAsync(AddAuthorDto author)
        {
            var result = await _authorService.AddAsync(author);
            if (result == null) { return BadRequest(); }
            return Ok(result);
        }

        [HttpPost("/author/update/{id:guid}")]
        public async Task<ActionResult<AuthorResponseDto>> UpdateAuthorAync([FromRoute] Guid id, [FromBody] AuthorResponseDto author)
        {
            if (id != author.Id)
            {
                return BadRequest("Id не совпадают");
            }
            var result = await _authorService.UpdateAsync(author);

            if (result == null) { return BadRequest(); }
            ;
            return Ok(result);
        }

        [HttpDelete("/author/delete/{id:guid}")]
        public async Task<IActionResult> DeleteAuthorAsync(Guid id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();

        }
    }
}
