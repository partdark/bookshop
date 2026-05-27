using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Infrastructure.Repositories
{


    public class OrdersRepository : IOrdersRepository
    {
        private readonly BookShopContext _context;
        private readonly HybridCache _cache;


        public OrdersRepository(BookShopContext context, HybridCache hybridCache)
        {
            _context = context;
            _cache = hybridCache;

        }

        public async Task<int> CreateAsync(AddOrderDto order)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var customerExists = await _context.Users.AnyAsync(c => c.Id == order.CustomerId);
                if (!customerExists)
                {
                    throw new ArgumentException($"Не удалось найти пользователя {order.CustomerId}");
                }
                var booksInOrder = await _context.Books
                    .Where(c => order.Items.Select(o => o.BookId).Contains(c.Id))
                    .ToDictionaryAsync(b => b.Id);

                if (booksInOrder.Count < order.Items.Count)
                {
                    throw new ArgumentException("Не все книги представлены на складе");
                }
                foreach (var item in order.Items)
                {
                    if (!booksInOrder.TryGetValue(item.BookId, out var book))
                    {
                        throw new ArgumentException($"Не все книги представлены на складе {item.BookId}");
                    }
                    if (item.Count > book.Count)
                        throw new ArgumentException($"Недостаточно книг для {book.Title}, " +
                            $"необходимо {item.Count} при наличии {book.Count}");
                }

                var newOrder = new Order()
                {
                    CustomerId = order.CustomerId,
                    CreatedDate = DateTime.UtcNow,
                    Status = OrderStatus.Placed,

                };

                await _context.Orders.AddAsync(newOrder);
                var priceSum = 0M;
                var itemsInOrder = new List<OrderItems>(order.Items.Count);
                foreach (var item in order.Items)
                {
                    booksInOrder[item.BookId].Count -= item.Count;

                    var orderItem = new OrderItems()
                    {
                        OrderId = newOrder.Id,
                        BookId = item.BookId,
                        Count = item.Count,
                        PriceAtPurchase = booksInOrder[item.BookId].Price,
                        Order = newOrder
                    };
                    priceSum += booksInOrder[item.BookId].Price * item.Count;
                    itemsInOrder.Add(orderItem);


                }

                newOrder.Items = itemsInOrder;
                newOrder.TotalPrice = priceSum;


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return newOrder.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Order> AddAsync(Order entity)
        {
            var order = await GetByIdAsync(entity.Id);
            if (order != null)
            {
                throw new ArgumentException($"order with ID {entity.Id} already exsits");
            }
            await _context.Orders.AddAsync(entity);
            await _context.SaveChangesAsync();
            await _cache.RemoveAsync("mainpage");
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var OrderCount = await _context.Orders.Where(o => o.Id == id).ExecuteDeleteAsync();

            if (OrderCount == 0)
            {
                return false;
            }


            return true;
        }



        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetDetailedByIdAsync(int id)
        {
            return await _context.Orders.AsNoTracking()
                .AsSplitQuery()
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }


        public async Task<List<int>> GetIdsAsync()
        {
            return await _context.Orders.AsNoTracking().Select(g => g.Id).ToListAsync();
        }

        public async Task<Order> UpdateAsync(Order entity)
        {
            _context.Orders.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
