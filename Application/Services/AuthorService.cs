using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;


namespace Application.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorsRepository _authorsRepository;

        public AuthorService(IAuthorsRepository authorsRepository)
        {
            _authorsRepository = authorsRepository;
        }

        public async Task<AuthorInfoDto?> GetByIdAsync(Guid Id)
        {
            var result = await _authorsRepository.GetByIdAsync(Id);

            if (result == null) { return null; }

            return new AuthorInfoDto(
                result.Id,
                result.Name,
                result.Year,
                result.Books.Select(b => (b.Id, b.Title)).ToList()
                );
        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _authorsRepository.GetIdsAsync();
        }

        public async Task<AuthorResponseDto?> AddAsync(AddAuthorDto author)
        {
            var authorToAdd = new Author() { Name = author.Name, Year = author.Year };
            var response = await _authorsRepository.AddAsync(authorToAdd);

            if (response == null)
            {
                return null;
            }
            return new AuthorResponseDto(response.Id, response.Name, response.Year);

        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _authorsRepository.DeleteAsync(id);
        }

        public async Task<AuthorResponseDto?> UpdateAsync(AuthorResponseDto authorToUpdate)
        {
            var data = await _authorsRepository.GetByIdAsync(authorToUpdate.Id);
            if (data == null)    { return null; }
            data.Name = authorToUpdate.Name;
            data.Year = authorToUpdate.Year;

            var result = await _authorsRepository.UpdateAsync(data);
            return new AuthorResponseDto(result.Id, result.Name, result.Year);



        }
    }


}
