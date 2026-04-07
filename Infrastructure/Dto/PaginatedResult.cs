namespace Infrastructure.Dto
{
    public partial class BooksRepository
    {
        public record PaginatedResult<T>(int TotalCount, int PageCount, int CurrentPage, bool HasNext, bool HasPrevious)
        {
            public List<T> Items { get; set; } = new();
        }
    }
}
