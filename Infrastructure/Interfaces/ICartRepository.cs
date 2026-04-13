using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface ICartRepository
    {
        Task<bool> AddItemToCart(Guid CustomerId, Guid BookId, int Count);
        Task<bool> ClearCart(Guid CustomerId);
        Task<Order?> CreateOrder(Guid CustomerId);
        Task<bool> CustomerNotExists(Guid id);
        Task<bool> DeleteItemFromInCart(Guid CustomerId, Guid BookId);
        Task<List<CartItem>?> GetCartItemsByCustomerId(Guid CustomerId);
        Task<bool> UpdateItemsCountInCart(Guid CustomerId, Guid BookId, int Count);
    }
}