using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService) => _genreService = genreService;

        [HttpGet("genres")]
        public async Task<ActionResult<List<GenreResponseDto>>> GetAll()
            => Ok(await _genreService.GetAll());

        [HttpGet("genre/{id:guid}")]
        public async Task<ActionResult<GenreResponseDto>> GetById(Guid id)
        {
            var genre = await _genreService.GetById(id);
            return genre == null ? NotFound() : Ok(genre);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("genre/add")]
        public async Task<ActionResult<Guid>> Add([FromBody] AddGenreDto genre)
            => Ok(await _genreService.Add(genre));

        [Authorize(Roles = "Admin")]
        [HttpPut("genre/update/{id:guid}")]
        public async Task<ActionResult<GenreResponseDto>> Update(Guid id, [FromBody] GenreResponseDto genre)
        {
            if (id != genre.id) return BadRequest("Id's do not match");
            var result = await _genreService.Update(genre);
            return result == null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("genre/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
            => await _genreService.Delete(id) ? Ok() : NotFound();
    }
}
