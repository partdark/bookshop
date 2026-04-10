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
        public  Task<bool> AddAsyncWithExistsAuthorAndGenres(Book entity, List<Guid> authors, List<Guid> genres);
        public  Task<List<Guid>> AddAuthorsFromBdToBook(Guid bookId, List<Guid> authors);



        public  Task<List<Guid>> AddGenresFromBdToBook(Guid bookId, List<Guid> genres);
        public Task<ListWithBooksBaseData> BooksBaseData(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool notAscending = false, string? searchingWords = null);
       public Task<PaginatedResult<Book>> TakeBookWithPagging(int pagesize = 20, int pageNumber = 1, string orderBy = "Title", bool ascending = true);
    }

    public interface IAuthorsRepository : IRepository<Author> { }

    public interface IGenresRepository : IRepository<Genre> { public Task<List<IdWithNAme>> GetIdsWithNamesAsync(); }

    public interface ICustomersRepository : IRepository<Customer> { public Task<List<IdWithNAme>> GetIdsWithNamesAsync(); }

    public interface IReviewsRepository : IRepository<Review> { public Task<List<Review>> GetAll(); }
    public interface IOrdersRepository
    {
        Task<Order> AddAsync(Order entity);
        Task<bool> DeleteAsync(int id);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
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
