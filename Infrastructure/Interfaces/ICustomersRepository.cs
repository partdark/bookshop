using Domain.Entities;
using Infrastructure.Dto;




namespace Infrastructure.Interfaces
{
    public interface ICustomersRepository : IRepository<Customer>
    {
        public Task<List<IdWithName>> GetIdsWithNamesAsync();
        public Task<Customer> AddAsync(Customer entity, string password);
    }


}
