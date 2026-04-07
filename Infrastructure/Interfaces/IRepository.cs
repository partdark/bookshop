using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static Infrastructure.Dto.BooksRepository;


namespace Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<T> AddAsync(T entity);
        public Task<bool> DeleteAsync(Guid id);
        public Task<T?> GetByIdAsync(Guid id);

        public Task<T> UpdateAsync(T entity);

        public Task<List<Guid>> GetIdsAsync();
    }

    public interface IBooksRepository : IRepository<Book>
    {
       
        Task<PaginatedResult<Book>> TakeBookWithPagging(int pagesize = 20, int pageNumber = 1, string orderBy = "Title", bool ascending = true);
    }

    public interface IAuthorsRepository : IRepository<Author> { }

    public interface IGenresRepository : IRepository<Genre> { }

    public interface ICustomersRepository : IRepository<Customer> { }

    public interface IReviewsRepository : IRepository<Review> { }
    public interface IOrdersRepository
    {
        Task<Order> AddAsync(Order entity);
        Task<bool> DeleteAsync(int id);
        Task<Order?> GetByIdAsync(int id);
        Task<List<int>> GetIdsAsync();
        Task<Order> UpdateAsync(Order entity);
    }

    
    public interface IOrderItemsRepository
    {
        Task<OrderItems> AddAsync(OrderItems entity);
        Task<bool> DeleteAsync(int orderId, Guid bookId);
        Task<OrderItems?> GetAsync(int orderId, Guid bookId);
        Task<List<OrderItems>> GetByOrderIdAsync(int orderId);
        Task<OrderItems> UpdateAsync(OrderItems entity);
    }


}
