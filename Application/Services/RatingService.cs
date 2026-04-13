using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class RatingService : IRatingService
    {
        private readonly BookShopContext _context;

        public RatingService(BookShopContext context)
        {
            _context = context;
        }

        public async Task RecalculateAsync(Guid bookId)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == bookId);

            if (book == null) return;

            book.Rating = book.Reviews.Count > 0
                ? (float)book.Reviews.Average(r => r.Rating)
                : 0f;

            await _context.SaveChangesAsync();
        }

        public async Task RecalculateAllAsync()
        {
            var books = await _context.Books
                .Include(b => b.Reviews)
                .ToListAsync();

            foreach (var book in books)
            {
                book.Rating = book.Reviews.Count > 0
                    ? (float)book.Reviews.Average(r => r.Rating)
                    : 0f;
            }

            await _context.SaveChangesAsync();
        }
    }
}
