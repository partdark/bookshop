using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet("/genres")]
        public async Task<ActionResult<List<GenreResponseDto>>> GetAll()
        {
            return Ok(await _genreService.GetAll());
        }

        [HttpGet("/genre/{id:guid}")]
        public async Task<ActionResult<GenreResponseDto>> GetById(Guid id)
        {
            var genre = await _genreService.GetById(id);
            if (genre == null)
            {
                return NotFound();
            }
            return Ok(genre);
        }

        [HttpPost("/genre/add")]
        public async Task<ActionResult<Guid>> Add([FromBody] AddGenreDto genre)
        {
            var id = await _genreService.Add(genre);
            return Ok(id);
        }

        [HttpPut("/genre/update/{id:guid}")]
        public async Task<ActionResult<GenreResponseDto>> Update(Guid id, [FromBody] GenreResponseDto genre)
        {
            if (id != genre.id)
            {
                return BadRequest("Id's do not match");
            }
            var updatedGenre = await _genreService.Update(genre);
            if (updatedGenre == null)
            {
                return NotFound();
            }
            return Ok(updatedGenre);
        }

        [HttpDelete("/genre/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _genreService.Delete(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
