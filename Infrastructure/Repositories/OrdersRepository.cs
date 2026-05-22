using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
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
        public async Task<Order> AddAsync(Order entity)
        {
            var order = await GetByIdAsync(entity.Id);
            if (order != null)
            {
                throw new ArgumentException($"order with ID {entity.Id} already exsits");
            }
            _context.Orders.Add(entity);
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
