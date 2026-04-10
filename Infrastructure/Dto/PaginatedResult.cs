namespace Infrastructure.Dto
{

    public record PaginatedResult<T>(int TotalCount, int PageCount, int CurrentPage, bool HasNext, bool HasPrevious)
    {
        public List<T> Items { get; set; } = new();
    }

    public record BookBaseData(Guid Id, string Title, string Description, 
    float Rating, decimal Price, string UrlImage, int Count, int PublicationYear);

    public record ListWithBooksBaseData (int lastNumber, bool hasNext, bool hasPrevious)
    {
        public List<BookBaseData> Books { get; set; } = new();
    }
    public record IdWithNAme(Guid Id, string Name);

}
