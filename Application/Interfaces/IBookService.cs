using Application.Dto;
using Domain.Entities;
using Infrastructure.Dto;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Interfaces
{
    public interface IBookService
    {
        public Task<AddBookDto?> PatchBook(Guid id, JsonPatchDocument<AddBookDto> patchBook);
        public Task<BookResponseDto> UpdateBook(BookResponseDto bookResponse);
        public Task<bool> DeleteAsync(Guid id);
        Task<Guid> AddBook(AddBookDto bookDto);
        BookResponseDto? ConvertToDto(Book book);
        Task<List<Guid>> GetBookSIds();
        Task<BookResponseDto?> GetById(Guid id);

        public Task<ListWithBooksBaseData> BookShowcase(int pageCapacity, int pageNumber, string orderBy,
           bool notAscending, string? searchingWords);

        public Task<Guid> CreateBookWithIndicatingExistingAuthorsAndgenres(AddBookDto bookDto,
           List<Guid> authorDto, List<Guid> genres);
    }
}