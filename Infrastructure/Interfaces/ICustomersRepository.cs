using Domain.Entities;
using Infrastructure.Dto;




namespace Infrastructure.Interfaces
{
    public interface ICustomersRepository : IRepository<Customer>
    {
        public Task<List<IdWithNAme>> GetIdsWithNamesAsync();
        new Task<Customer> AddAsync(Customer entity, string password);
    }


}
