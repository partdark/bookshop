using Domain.Entities;
using Infrastructure.Dto;




namespace Infrastructure.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public Task<T?> AddAsync(T entity);
        public Task<bool> DeleteAsync(Guid id);
        public Task<T?> GetByIdAsync(Guid id);

        public Task<T> UpdateAsync(T entity);

        public Task<List<Guid>> GetIdsAsync();
    }

    public interface IBooksRepository : IRepository<Book>
    {
        Task<bool> AddAsyncWithExistsAuthorAndGenres(Book entity, List<Guid> authors, List<Guid> genres);
        Task<List<Guid>> AddAuthorsFromBdToBook(Guid bookId, List<Guid> authors);
        Task<List<Guid>> AddGenresFromBdToBook(Guid bookId, List<Guid> genres);
        Task<ListWithBooksBaseData> BooksBaseData(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool notAscending = false, string? searchingWords = null);
        Task<PaginatedResult<Book>> TakeBookWithPagging(int pagesize = 20, int pageNumber = 1, string orderBy = "Title", bool ascending = true);
        Task UpdateCountAsync(Guid id, int count);
    }

    public interface IAuthorsRepository : IRepository<Author> { }

    public interface IGenresRepository : IRepository<Genre> { public Task<List<IdWithNAme>> GetIdsWithNamesAsync(); }

    public interface ICustomersRepository : IRepository<Customer>
    {
        public Task<List<IdWithNAme>> GetIdsWithNamesAsync();
        new Task<Customer> AddAsync(Customer entity, string password);
    }

    public interface IReviewsRepository : IRepository<Review>
    {
        Task<List<Review>> GetAll();
        Task<Review?> GetByCustomerAndBookAsync(Guid customerId, Guid bookId);
    }
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
