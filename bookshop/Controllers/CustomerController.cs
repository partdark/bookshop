using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CustomerController(ICustomerService customerService, IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerResponseIdNameDto>>> GetAll()
        {
            return Ok(await _customerService.GetAll());
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<CustomerResponseDto>> GetById(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || currentUserId != id.ToString())
                return Forbid();

            var customer = await _customerService.GetById(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<ActionResult<CustomerResponseDto>> Update(Guid id, [FromBody] UpdateCustomerDto customer)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || currentUserId != id.ToString())
                return Forbid();

            var updatedCustomer = await _customerService.Update(id, customer);
            if (updatedCustomer == null) return NotFound();
            return Ok(updatedCustomer);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || currentUserId != id.ToString())
                return Forbid();

            var result = await _customerService.Delete(id);
            if (result) return Ok();
            return NotFound();
        }

        [HttpGet("{id:guid}/orders")]
        [Authorize]
        public async Task<ActionResult<List<OrderResponseDto>>> GetMyOrders(Guid id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || currentUserId != id.ToString())
                return Forbid();

            var orders = await _orderService.GetByCustomerId(id);
            return Ok(orders);
        }
    }
}
