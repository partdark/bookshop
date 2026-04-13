using Domain.Entities;

namespace Infrastructure.Interfaces
{
    public interface ICartRepository
    {
        Task<List<CartItem>?> GetCartItemsByCustomerId(Guid customerId);
        Task<bool> AddItemToCart(Guid customerId, Guid bookId, int count);
        Task<bool> UpdateItemsCountInCart(Guid customerId, Guid bookId, int count);
        Task<bool> DeleteItemFromInCart(Guid customerId, Guid bookId);
        Task<bool> ClearCart(Guid customerId);
        Task<Order?> CreateOrder(Guid customerId);
    }
}
