using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public  class ReviewsRepository : IReviewsRepository
    {
        private readonly BookShopContext _context;

        public ReviewsRepository(BookShopContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Reviews.AsNoTracking().Select(g => g.Id).ToListAsync();
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
            if (review == null) return false;
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Review> AddAsync(Review entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)}");
            }
            if (await GetByIdAsync(entity.Id) != null)
            {
                throw new InvalidOperationException($"Book with ID{entity.Id} exists");
            }
            _context.Reviews.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Review> UpdateAsync(Review entity)
        {

            _context.Reviews.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<Review>> GetAll()
        {
            return await _context.Reviews
                .AsNoTracking()
                .Include(r => r.Customer)
                .Include(r => r.Book)
                .ToListAsync();
        }

        public async Task<Review?> GetByCustomerAndBookAsync(Guid customerId, Guid bookId)
        {
            return await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.CustomerId == customerId && r.BookId == bookId);
        }
    }
}

