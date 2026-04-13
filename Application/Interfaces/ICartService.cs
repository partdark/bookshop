using Application.Dto;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItemDto>?> GetCartItemsByCustomerId(Guid customerId);
        Task<bool> AddItemToCart(Guid customerId, Guid bookId, int count);
        Task<bool> UpdateItemsCountInCart(Guid customerId, Guid bookId, int count);
        Task<bool> DeleteItemFromInCart(Guid customerId, Guid bookId);
        Task<bool> ClearCart(Guid customerId);
        Task<OrderResponseDto?> CreateOrder(Guid customerId);
    }
}
