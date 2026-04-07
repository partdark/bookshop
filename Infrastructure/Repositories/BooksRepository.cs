using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Dynamic.Core;
using static Infrastructure.Dto.BooksRepository;

namespace Infrastructure.Repositories
{
    public partial class BooksRepository : IBooksRepository
    {
        private readonly BookShopContext _context;


        public BooksRepository(BookShopContext context)
        {
            _context = context;
        }



        public async Task<Book?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Books.AsNoTracking()
                  .Include(a => a.Authors)
                  .Include(g => g.Genres)
                  .AsSingleQuery()
                  .FirstOrDefaultAsync(b => b.Id == id);

            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            _context.Books.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<Book>> TakeBookWithPagging(int pageSize = 20, int pageNumber = 1, string orderBy = "title", bool ascending = true)
        {

            if (pageSize <= 0) pageSize = 20;
            if (pageNumber <= 0) pageNumber = 1;

            var q = _context.Books.AsNoTracking()
                   .Include(a => a.Authors)
                   .Include(g => g.Genres)
                   .Include(r => r.Reviews)
                   .AsQueryable();
            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderString = $"{orderBy} {(ascending ? "asc" : "desc")}";
                q = q.OrderBy(orderString);
            }
            else
            {
                q = q.OrderBy(t => t.Title);
            }
            var total = await q.CountAsync();
            var pages = (int)Math.Ceiling((double)total / pageSize);
            if (pages == 0)
            {
                return new PaginatedResult<Book>(default, default, default, default, default)
                {
                    Items = new List<Book>(),
                    TotalCount = 0,
                    PageCount = 0,
                    CurrentPage = 0,
                    HasNext = false,
                    HasPrevious = false,
                };
            }
            pageNumber = Math.Min(pageNumber, pages);


            q = q.Skip(pageSize * (pageNumber - 1)).Take(pageSize);

            var items = await q.ToListAsync();

            return new PaginatedResult<Book>(default, default, default, default, default)
            {
                Items = items,
                TotalCount = total,
                PageCount = pages,
                CurrentPage = pageNumber,
                HasNext = pageNumber < pages,
                HasPrevious = pageNumber > 1,

            };


        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Books.AsNoTracking().Select(b => b.Id).ToListAsync();
        }

        public async Task<Book> UpdateAsync(Book entity)
        {


            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Book> AddAsync(Book entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)}");
            }
            if (await GetByIdAsync(entity.Id) != null)
            {
                throw new InvalidOperationException($"Book with ID{entity.Id} exists");
            }
            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

      
    }
}
