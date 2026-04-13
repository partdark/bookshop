using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {


        private readonly BookShopContext _context;


        public CartRepository(BookShopContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>?> GetCartItemsByCustomerId(Guid CustomerId)
        {

            if (!await CustomerNotExists(CustomerId))
            {
                throw new KeyNotFoundException($"Cant find customer {CustomerId}");
            }

            var result = await _context.CartItems.AsNoTracking()
                .Include(x => x.Book)
                .AsSingleQuery()
                .Where(x => x.CustomerId == CustomerId).ToListAsync();

            return result;
        }

        public async Task<bool> AddItemToCart(Guid CustomerId, Guid BookId, int Count)
        {
            if (!await CustomerNotExists(CustomerId))
            {
                throw new KeyNotFoundException($"Cant find customer {CustomerId}");
            }

            var newCartItem = new CartItem
            {
                BookId = BookId,
                Quantity = Count,

            };
            await _context.CartItems.AddAsync(newCartItem);
            await _context.SaveChangesAsync();

            return true;


        }

        public async Task<bool> UpdateItemsCountInCart(Guid CustomerId, Guid BookId, int Count)
        {
            if (!await CustomerNotExists(CustomerId))
            {
                throw new KeyNotFoundException($"Cant find customer {CustomerId}");
            }
            var items = await _context.CartItems.Where(c => c.CustomerId == CustomerId && c.BookId == BookId)
                .ExecuteUpdateAsync(s => s.SetProperty(q => q.Quantity, Count));

            if (items == 1)
            {
                return true;
            }
            return false;

        }
        public async Task<bool> DeleteItemFromInCart(Guid CustomerId, Guid BookId)
        {
            if (!await CustomerNotExists(CustomerId))
            {
                throw new KeyNotFoundException($"Cant find customer {CustomerId}");
            }
            var items = await _context.CartItems.Where(c => c.CustomerId == CustomerId && c.BookId == BookId)
                .ExecuteDeleteAsync();

            if (items == 1)
            {
                return true;
            }
            return false;

        }

        public async Task<bool> ClearCart(Guid CustomerId)
        {
            {
                if (!await CustomerNotExists(CustomerId))
                {
                    throw new KeyNotFoundException($"Cant find customer {CustomerId}");
                }
                var items = await _context.CartItems.Where(c => c.CustomerId == CustomerId)
                    .ExecuteDeleteAsync();

                if (items >= 0)
                {
                    return true;
                }
                return false;

            }
        }

        public async Task<Order?> CreateOrder(Guid CustomerId)
        {
            var cartItems = await GetCartItemsByCustomerId(CustomerId);
            {
                if (cartItems == null || cartItems.Count == 0)
                {
                    return null;
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    CustomerId = CustomerId,
                };
                var orderItems = new List<OrderItems>();
                var totalPrice = 0M;
                var booksIds = cartItems.Select(c => c.BookId).Distinct().ToList();
                var prices = await _context.Books.Where(b => booksIds.Contains(b.Id)).ToDictionaryAsync(b => b.Id, b => b.Price);

                foreach (var item in cartItems)
                {
                    if (!prices.TryGetValue(item.BookId, out var price))
                    {
                        throw new ArgumentException($"Книга не найдена {item.BookId}");
                    }
                    var orderItem = new OrderItems
                    {
                        OrderId = order.Id,
                        BookId = item.BookId,
                        Count = item.Quantity,
                        PriceAtPurchase = price,

                    };
                    orderItems.Add(orderItem);
                    totalPrice += price * item.Quantity;
                }
                order.TotalPrice = totalPrice;
                await _context.Orders.AddAsync(order);
                await _context.OrderItems.AddRangeAsync(orderItems);
                await ClearCart(CustomerId);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return order;
            }
            catch (Exception ex)
            {
                transaction.Rollback(); throw;
            }



        }


        public async Task<bool> CustomerNotExists(Guid id)
        {
            var customer = await _context.Users.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return false;
            }
            return true;
        }

    }
}
