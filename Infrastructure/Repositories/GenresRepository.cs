using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly BookShopContext _context;

        public GenresRepository(BookShopContext context)
        {
            _context = context;
        }

        public async Task<Genre?> GetByIdAsync(Guid id)
        {
            var genre = _context.Genres.AsNoTracking().FirstOrDefault(g => g.Id == id);

            return genre;
        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Genres.AsNoTracking().Select(g => g.Id).ToListAsync();
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var genre = await GetByIdAsync(id);

            if (genre == null)
            {
                throw new InvalidOperationException($"Cant find Genre with ID:{id}");
            }
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Genre> AddAsync(Genre entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)}");
            }
            if (await GetByIdAsync(entity.Id) != null)
            {
                throw new InvalidOperationException($"Genre with ID{entity.Id} exists");
            }
            _context.Genres.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Genre> UpdateAsync(Genre entity)
        {
          
            _context.Genres.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }

}

