using Application.Dto;

namespace Application.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorResponseDto?> AddAsync(AddAuthorDto author);
        Task<bool> DeleteAsync(Guid id);
        Task<AuthorInfoDto?> GetByIdAsync(Guid Id);
        Task<List<Guid>> GetIdsAsync();
        Task<List<AuthorResponseDto>> GetAllAsync();
        Task<AuthorResponseDto?> UpdateAsync(AuthorResponseDto authorToUpdate);
    }
}