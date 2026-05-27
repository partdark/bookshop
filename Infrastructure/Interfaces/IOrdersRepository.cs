using Domain.Entities;
using Infrastructure.Dto;




namespace Infrastructure.Interfaces
{
    public interface IOrdersRepository
    {
        Task<Order> AddAsync(Order entity);
        Task<bool> DeleteAsync(int id);
        Task<Order?> GetByIdAsync(int id);
        Task<Order?> GetDetailedByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task<List<int>> GetIdsAsync();
        Task<Order> UpdateAsync(Order entity);
        Task<List<Order>> GetByCustomerIdAsync(Guid customerId);

       Task<int> CreateAsync(AddOrderDto order);
    }


}
