using Domain.Entities;
using Infrastructure.Dto;




namespace Infrastructure.Interfaces
{
    public interface IBooksRepository : IRepository<Book>
    {
        Task<bool> AddAsyncWithExistsAuthorAndGenres(Book entity, List<Guid> authors, List<Guid> genres);
        Task<List<Guid>> AddAuthorsFromBdToBook(Guid bookId, List<Guid> authors);
        Task<List<Guid>> AddGenresFromBdToBook(Guid bookId, List<Guid> genres);
        Task<ListWithBooksBaseData> BooksBaseData(int pageCapacity = 20, int pageNumber = 1, string orderBy = "Title",
            bool notAscending = false, string? searchingWords = null, bool countMoreThenZero = true);
        Task<Dtos<Book>> TakeBookWithPagging(int pagesize = 20, int pageNumber = 1, string orderBy = "Title", bool ascending = true);
        Task UpdateCountAsync(Guid id, int count);
        Task PatchScalarFieldsAsync(Guid id, string title, string description, float rating, decimal price, string urlImage, int count, int publicationYear);
    }


}
