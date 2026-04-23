using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bookshop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(List<CartItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCartItems(Guid customerId)
        {
            var cartItems = await _cartService.GetCartItemsByCustomerId(customerId);
            if (cartItems == null)
            {
                return NotFound($"Cart for customer {customerId} not found.");

            }
            
            return Ok(cartItems);
        }

        [HttpPost("{customerId}/add")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddItemToCart(Guid customerId, [FromQuery] Guid bookId, [FromQuery] int count)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than zero.");
            }
            var result = await _cartService.AddItemToCart(customerId, bookId, count);
            if (!result)
            {
                return BadRequest("Could not add item to cart.");
            }
            return Ok(result);
        }

        [HttpPut("{customerId}/update")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateItemsCountInCart(Guid customerId, [FromQuery] Guid bookId, [FromQuery] int count)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than zero.");
            }
            var result = await _cartService.UpdateItemsCountInCart(customerId, bookId, count);
            if (!result)
            {
                return BadRequest("Could not update item count in cart.");
            }
            return Ok(result);
        }

        [HttpDelete("{customerId}/remove/{bookId}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteItemFromInCart(Guid customerId, Guid bookId)
        {
            var result = await _cartService.DeleteItemFromInCart(customerId, bookId);
            if (!result)
            {
                return BadRequest("Could not remove item from cart.");
            }
            return Ok(result);
        }

        [HttpDelete("{customerId}/clear")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ClearCart(Guid customerId)
        {
            var result = await _cartService.ClearCart(customerId);
            if (!result)
            {
                return BadRequest("Could not clear cart.");
            }
            return Ok(result);
        }

        [HttpPost("{customerId}/checkout")]
        [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Checkout(Guid customerId)
        {
            var order = await _cartService.CreateOrder(customerId);
            if (order == null)
            {
                return BadRequest("Could not create order. Cart might be empty or customer not found.");
            }
            return Ok(order);
        }
    }
}
