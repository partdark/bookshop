using Domain.Entities;




namespace Infrastructure.Interfaces
{
    public interface IReviewsRepository : IRepository<Review>
    {
        Task<List<Review>> GetAll();
        Task<Review?> GetByCustomerAndBookAsync(Guid customerId, Guid bookId);
    }


}
