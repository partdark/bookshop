using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class OrderItemsRepository : IOrderItemsRepository
    {
        private readonly BookShopContext _context;
        public OrderItemsRepository(BookShopContext context)
        {
            _context = context;
        }

        public async Task<OrderItems> AddAsync(OrderItems entity)
        {
            _context.OrderItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int orderId, Guid bookId)
        {
           var order = await GetAsync(orderId, bookId);
            if (order == null) { return false; }    
            _context.OrderItems.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<OrderItems?> GetAsync(int orderId, Guid bookId)
        {
            return await _context.OrderItems.AsNoTracking().FirstOrDefaultAsync(i => i.OrderId == orderId && i.BookId == bookId);
        }

        public async Task<List<OrderItems>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems.AsNoTracking().Where(i => i.OrderId == orderId).ToListAsync();
        }

        public async Task<OrderItems> UpdateAsync(OrderItems entity)
        {
           _context.OrderItems.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
