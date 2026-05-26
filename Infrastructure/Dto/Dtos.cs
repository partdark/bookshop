using System.Text.Json.Serialization;

namespace Infrastructure.Dto
{
    public record Dtos<T>(int TotalCount, int PageCount, int CurrentPage, bool HasNext, bool HasPrevious)
    {
        public List<T> Items { get; set; } = new();
    }

    public record BookAuthorData(Guid Id, string Name, int Year);
    public record BookGenreData(Guid Id, string Name);

    public record BookBaseData(
        Guid Id, string Title, string Description,
        float Rating, decimal Price, string UrlImage,
        int Count, int PublicationYear,
        List<BookAuthorData> Authors,
        List<BookGenreData> Genres);

    public record ListWithBooksBaseData(int TotalCount, int PageNumber, int PageCapacity, bool HasNext, bool HasPrevious)
    {
      

        [JsonInclude]
        public List<BookBaseData> Books { get; set; } = new();
    }

    public record IdWithName(Guid Id, string Name);

    public record ReportOrderCount(string Name, int Count);

    public record ReportOrderMoney(string Name, int Count, decimal TotalMoney);
}
