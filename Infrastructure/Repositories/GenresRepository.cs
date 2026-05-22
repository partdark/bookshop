using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


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
            var genre =  await _context.Genres.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);

            return genre;
        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Genres.AsNoTracking().Select(g => g.Id).ToListAsync();
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var genreCount = await _context.Genres.Where(g => g.Id == id).ExecuteDeleteAsync();

            if (genreCount == 0)
            {
                throw new InvalidOperationException($"Cant find Genre with ID:{id}");
            }
            return true;
        }

        public async Task<Genre?> AddAsync(Genre entity)
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

        public async Task<List<IdWithName>> GetIdsWithNamesAsync()
        {
            return await _context.Genres.Select(g => new IdWithName (g.Id, g.Name)).ToListAsync();
        }
    }

}

