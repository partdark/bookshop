using Application.Dto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGenreService
    {
        Task<List<GenreResponseDto>> GetAll();
        Task<GenreResponseDto?> GetById(Guid id);
        Task<Guid> Add(AddGenreDto genre);
        Task<GenreResponseDto?> Update(GenreResponseDto genre);
        Task<bool> Delete(Guid id);
    }
}
