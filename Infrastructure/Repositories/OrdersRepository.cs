using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{


    public class OrdersRepository : IOrdersRepository
    {
        private readonly BookShopContext _context;

        public OrdersRepository(BookShopContext context)
        {
            _context = context;
        }
        public async Task<Order> AddAsync(Order entity)
        {
            var order = await GetByIdAsync(entity.Id);
            if (order != null)
            {
                throw new ArgumentException($"order with ID {entity.Id} already exsits");
            }
            _context.Orders.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await GetByIdAsync(id);
            if (order == null) { return false; }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }



        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);
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
