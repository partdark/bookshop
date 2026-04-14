using Domain.Entities;




namespace Infrastructure.Interfaces
{
    public interface IOrderItemsRepository
    {
        Task<OrderItems> AddAsync(OrderItems entity);
        Task<bool> DeleteAsync(int orderId, Guid bookId);
        Task<OrderItems?> GetAsync(int orderId, Guid bookId);
        Task<List<OrderItems>> GetByOrderIdAsync(int orderId);
        Task<OrderItems> UpdateAsync(OrderItems entity);

        
    }


}
