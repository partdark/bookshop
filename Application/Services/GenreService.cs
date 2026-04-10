using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenresRepository _genresRepository;

        public GenreService(IGenresRepository genresRepository)
        {
            _genresRepository = genresRepository;
        }

        public async Task<Guid> Add(AddGenreDto genreDto)
        {
            var genre = new Genre
            {
                Name = genreDto.Name
            };
            await _genresRepository.AddAsync(genre);
            return genre.Id;
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _genresRepository.DeleteAsync(id);
        }

        public async Task<List<GenreResponseDto>> GetAll()
        {
            var genres = await _genresRepository.GetIdsWithNamesAsync();
            return genres.Select(g => new GenreResponseDto(g.Id, g.Name)).ToList();
        }

        public async Task<GenreResponseDto?> GetById(Guid id)
        {
            var genre = await _genresRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return null;
            }
            return new GenreResponseDto(genre.Id, genre.Name);
        }

        public async Task<GenreResponseDto?> Update(GenreResponseDto genreDto)
        {
            var genre = new Genre
            {
                Id = genreDto.id,
                Name = genreDto.Name
            };
            var updatedGenre = await _genresRepository.UpdateAsync(genre);
            if(updatedGenre == null)
            {
                return null;
            }
            return new GenreResponseDto(updatedGenre.Id, updatedGenre.Name);
        }
    }
}
