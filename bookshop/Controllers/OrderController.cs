using Application.Dto;
using Application.Interfaces;
using Infrastructure.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) => _orderService = orderService;

        [Authorize(Roles = "Admin")]
        [HttpGet("orders")]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
            => Ok(await _orderService.GetAll());

        [Authorize]
        [HttpGet("order/{id:int}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(int id)
        {
            var order = await _orderService.GetById(id);
            return order == null ? NotFound() : Ok(order);
        }

        [Authorize]
        [HttpGet("order/{id:int}/detail")]
        public async Task<ActionResult<OrderDetailDto>> GetDetailedById(int id)
        {
            var order = await _orderService.GetDetailedById(id);
            return order == null ? NotFound() : Ok(order);
        }
        [Authorize]
        [HttpPost("order/add")]
        public async Task<ActionResult<int>> Add([FromBody] AddOrderDto order)
        {
            try { return Ok(await _orderService.Add(order)); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("order/{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateStatus(id, dto.Status);
            return result ? Ok() : NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("order/delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
            => await _orderService.Delete(id) ? Ok() : NotFound();
    }
}
