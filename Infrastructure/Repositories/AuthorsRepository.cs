using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class AuthorsRepository : IAuthorsRepository
    {
        private readonly BookShopContext _context;

        public AuthorsRepository(BookShopContext context)
        {
            _context = context;
        }

        public async Task<Author?> GetByIdAsync(Guid id)
        {
            var author = await _context.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            return author;
        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Authors.AsNoTracking().Select(a => a.Id).ToListAsync();

        }

        public async Task<Author> AddAsync(Author author)
        {
            if (await GetByIdAsync(author.Id) != null)
            {
                throw new InvalidOperationException($"Author with ID:{author.Id} exists");
            }

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var author = await GetByIdAsync(id);

            if (author == null)
            {
                return false;
            }
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;


        }


        public async Task<Author> UpdateAsync(Author entity)
        {
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
