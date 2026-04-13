using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService) => _reviewService = reviewService;

        [HttpGet("reviews")]
        public async Task<ActionResult<List<ReviewResponseDto>>> GetAll()
            => Ok(await _reviewService.GetAll());

        [HttpGet("review/{id:guid}")]
        public async Task<ActionResult<ReviewResponseDto>> GetById(Guid id)
        {
            var review = await _reviewService.GetById(id);
            return review == null ? NotFound() : Ok(review);
        }

        [HttpPost("review/add")]
        public async Task<ActionResult<Guid>> Add([FromBody] AddReviewDto review)
        {
            try { return Ok(await _reviewService.Add(review)); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("review/update/{id:guid}")]
        public async Task<ActionResult<ReviewResponseDto>> Update(Guid id, [FromBody] UpdateReviewDto review)
        {
            if (id != review.Id) return BadRequest("Id's do not match");
            var result = await _reviewService.Update(review);
            return result == null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("review/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
            => await _reviewService.Delete(id) ? Ok() : NotFound();
    }
}
