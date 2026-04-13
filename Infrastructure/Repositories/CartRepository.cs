using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly BookShopContext _context;

        public CartRepository(BookShopContext context) => _context = context;

        public async Task<List<CartItem>?> GetCartItemsByCustomerId(Guid customerId)
        {
            if (!await CustomerExists(customerId))
                throw new KeyNotFoundException($"Customer {customerId} not found");

            return await _context.CartItems
                .AsNoTracking()
                .Include(x => x.Book)
                    .ThenInclude(b => b.Authors)
                .Include(x => x.Book)
                    .ThenInclude(b => b.Genres)
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<bool> AddItemToCart(Guid customerId, Guid bookId, int count)
        {
            if (!await CustomerExists(customerId))
                throw new KeyNotFoundException($"Customer {customerId} not found");

            // Если уже есть — увеличиваем количество
            var existing = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.BookId == bookId);

            if (existing != null)
            {
                existing.Quantity += count;
                await _context.SaveChangesAsync();
                return true;
            }

            await _context.CartItems.AddAsync(new CartItem
            {
                CustomerId = customerId,   // ← был баг: не задавался
                BookId = bookId,
                Quantity = count,
                AddedAt = DateTime.UtcNow,
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateItemsCountInCart(Guid customerId, Guid bookId, int count)
        {
            if (!await CustomerExists(customerId))
                throw new KeyNotFoundException($"Customer {customerId} not found");

            var updated = await _context.CartItems
                .Where(c => c.CustomerId == customerId && c.BookId == bookId)
                .ExecuteUpdateAsync(s => s.SetProperty(q => q.Quantity, count));

            return updated == 1;
        }

        public async Task<bool> DeleteItemFromInCart(Guid customerId, Guid bookId)
        {
            if (!await CustomerExists(customerId))
                throw new KeyNotFoundException($"Customer {customerId} not found");

            var deleted = await _context.CartItems
                .Where(c => c.CustomerId == customerId && c.BookId == bookId)
                .ExecuteDeleteAsync();

            return deleted == 1;
        }

        public async Task<bool> ClearCart(Guid customerId)
        {
            if (!await CustomerExists(customerId))
                throw new KeyNotFoundException($"Customer {customerId} not found");

            await _context.CartItems
                .Where(c => c.CustomerId == customerId)
                .ExecuteDeleteAsync();

            return true;
        }

        public async Task<Order?> CreateOrder(Guid customerId)
        {
            var cartItems = await _context.CartItems
                .AsNoTracking()
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            if (cartItems.Count == 0) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var bookIds = cartItems.Select(c => c.BookId).Distinct().ToList();
                var books = await _context.Books
                    .Where(b => bookIds.Contains(b.Id))
                    .ToDictionaryAsync(b => b.Id, b => b);

                // Проверяем наличие
                foreach (var item in cartItems)
                {
                    if (!books.TryGetValue(item.BookId, out var book))
                        throw new ArgumentException($"Книга не найдена: {item.BookId}");
                    if (book.Count < item.Quantity)
                        throw new ArgumentException($"Недостаточно экземпляров книги «{book.Title}»: доступно {book.Count}, запрошено {item.Quantity}.");
                }

                var order = new Order { CustomerId = customerId };
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                var orderItems = cartItems.Select(item =>
                {
                    var price = books[item.BookId].Price;
                    return new OrderItems
                    {
                        OrderId = order.Id,
                        BookId = item.BookId,
                        Count = item.Quantity,
                        PriceAtPurchase = price,
                    };
                }).ToList();

                order.TotalPrice = orderItems.Sum(i => i.PriceAtPurchase * i.Count);
                await _context.OrderItems.AddRangeAsync(orderItems);

                // Списываем количество
                foreach (var item in cartItems)
                {
                    var newCount = books[item.BookId].Count - item.Quantity;
                    await _context.Books
                        .Where(b => b.Id == item.BookId)
                        .ExecuteUpdateAsync(s => s.SetProperty(b => b.Count, newCount));
                }

                // Очищаем корзину
                await _context.CartItems
                    .Where(c => c.CustomerId == customerId)
                    .ExecuteDeleteAsync();

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<bool> CustomerExists(Guid id) =>
            await _context.Users.AnyAsync(c => c.Id == id);
    }
}
