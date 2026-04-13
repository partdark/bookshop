using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;

namespace Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBookService _bookService;

        public CartService(ICartRepository cartRepository, IBookService bookService)
        {
            _cartRepository = cartRepository;
            _bookService = bookService;
        }

        public async Task<List<CartItemDto>?> GetCartItemsByCustomerId(Guid customerId)
        {
            var cartItems = await _cartRepository.GetCartItemsByCustomerId(customerId);
            if (cartItems == null || !cartItems.Any())
            {
                return null;
            }

            var cartItemDtos = new List<CartItemDto>();
            foreach (var item in cartItems)
            {
                var bookDto = _bookService.ConvertToDto(item.Book);
                if (bookDto != null)
                {
                    cartItemDtos.Add(new CartItemDto(bookDto, item.Quantity));
                }
            }
            return cartItemDtos;
        }

        public async Task<bool> AddItemToCart(Guid customerId, Guid bookId, int count)
        {
            return await _cartRepository.AddItemToCart(customerId, bookId, count);
        }

        public async Task<bool> UpdateItemsCountInCart(Guid customerId, Guid bookId, int count)
        {
            return await _cartRepository.UpdateItemsCountInCart(customerId, bookId, count);
        }

        public async Task<bool> DeleteItemFromInCart(Guid customerId, Guid bookId)
        {
            return await _cartRepository.DeleteItemFromInCart(customerId, bookId);
        }

        public async Task<bool> ClearCart(Guid customerId)
        {
            return await _cartRepository.ClearCart(customerId);
        }

        public async Task<OrderResponseDto?> CreateOrder(Guid customerId)
        {
            var order = await _cartRepository.CreateOrder(customerId);
            if (order == null) return null;

            var orderItemDtos = order.Items?
                .Select(oi => new OrderItemDto(oi.BookId, oi.Count, oi.PriceAtPurchase))
                .ToList() ?? new List<OrderItemDto>();

            return new OrderResponseDto(
                order.Id, order.CustomerId, order.CreatedDate,
                order.TotalPrice, order.Status.ToString(), orderItemDtos);
        }
    }
}
