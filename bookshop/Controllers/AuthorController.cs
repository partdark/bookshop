using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService authorService) => _authorService = authorService;

        [HttpGet("author/{id:guid}")]
        public async Task<ActionResult<AuthorInfoDto>> GetById(Guid id)
        {
            var result = await _authorService.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("authors")]
        public async Task<ActionResult<List<Guid>>> GetIds()
            => Ok(await _authorService.GetIdsAsync());

        [HttpGet("authors/all")]
        public async Task<ActionResult<List<AuthorResponseDto>>> GetAll()
            => Ok(await _authorService.GetAllAsync());

        [Authorize(Roles = "Admin")]
        [HttpPut("author/add")]
        public async Task<ActionResult<AuthorResponseDto>> Add(AddAuthorDto author)
        {
            var result = await _authorService.AddAsync(author);
            return result == null ? BadRequest() : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("author/update/{id:guid}")]
        public async Task<ActionResult<AuthorResponseDto>> Update([FromRoute] Guid id, [FromBody] AuthorResponseDto author)
        {
            if (id != author.Id) return BadRequest("Id не совпадают");
            var result = await _authorService.UpdateAsync(author);
            return result == null ? BadRequest() : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("author/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
            => await _authorService.DeleteAsync(id) ? Ok() : NotFound();
    }
}
