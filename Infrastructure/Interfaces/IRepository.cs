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


}
