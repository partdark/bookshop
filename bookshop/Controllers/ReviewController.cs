using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("/reviews")]
        public async Task<ActionResult<List<ReviewResponseDto>>> GetAll()
        {
            return Ok(await _reviewService.GetAll());
        }

        [HttpGet("/review/{id:guid}")]
        public async Task<ActionResult<ReviewResponseDto>> GetById(Guid id)
        {
            var review = await _reviewService.GetById(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        [HttpPost("/review/add")]
        public async Task<ActionResult<Guid>> Add([FromBody] AddReviewDto review)
        {
            try
            {
                var id = await _reviewService.Add(review);
                return Ok(id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("/review/update/{id:guid}")]
        public async Task<ActionResult<ReviewResponseDto>> Update(Guid id, [FromBody] UpdateReviewDto review)
        {
            if (id != review.Id)
            {
                return BadRequest("Id's do not match");
            }
            var updatedReview = await _reviewService.Update(review);
            if (updatedReview == null)
            {
                return NotFound();
            }
            return Ok(updatedReview);
        }

        [HttpDelete("/review/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _reviewService.Delete(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
