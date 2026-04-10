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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("/orders")]
        public async Task<ActionResult<List<OrderResponseDto>>> GetAll()
        {
            return Ok(await _orderService.GetAll());
        }

        [HttpGet("/order/{id:int}")]
        public async Task<ActionResult<OrderResponseDto>> GetById(int id)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost("/order/add")]
        public async Task<ActionResult<int>> Add([FromBody] AddOrderDto order)
        {
            try
            {
                var id = await _orderService.Add(order);
                return Ok(id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/order/delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _orderService.Delete(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
