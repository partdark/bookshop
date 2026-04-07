using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly BookShopContext _context;
        public CustomersRepository(BookShopContext context)
        {
            _context = context;
        }
        public async Task<Customer> AddAsync(Customer entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(entity)}");
            }
            if (await GetByIdAsync(entity.Id) != null)
            {
                throw new InvalidOperationException($"Customer with ID{entity.Id} exists");
            }
            _context.Customers.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            _context.Customers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Customers.AsNoTracking()
                  .Include(o => o.Orders)
                  .Include(r => r.Reviews)
                  .AsSingleQuery()
                  .FirstOrDefaultAsync(b => b.Id == id);

            return entity;
        }

        public  async   Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Customers.AsNoTracking().Select(b => b.Id).ToListAsync();
        }

        public async Task<Customer> UpdateAsync(Customer entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
