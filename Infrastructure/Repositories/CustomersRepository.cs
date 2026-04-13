using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly BookShopContext _context;
        private readonly UserManager<Customer> _userManager;
        public CustomersRepository(BookShopContext context, UserManager<Customer> userManager)
        {
            _context = context;
            _userManager = userManager;
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
            var roleResult = await _userManager.CreateAsync(entity, "User");
            if (!roleResult.Succeeded)
            {
                throw new OperationCanceledException($"Не удалось применить роль для {entity.Id} {entity.UserName}");
            }

            _context.Users.Add(entity);
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
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Users.AsNoTracking()
                  .Include(o => o.Orders)
                  .Include(r => r.Reviews)
                  .AsSingleQuery()
                  .FirstOrDefaultAsync(b => b.Id == id);

            return entity;
        }

        public async Task<List<Guid>> GetIdsAsync()
        {
            return await _context.Users.AsNoTracking().Select(b => b.Id).ToListAsync();
        }

        public async Task<List<IdWithNAme>> GetIdsWithNamesAsync()
        {
            return await _context.Users.AsNoTracking().Select(b => new IdWithNAme ( b.Id, b.UserName )).ToListAsync();
           
        }

        public async Task<Customer> UpdateAsync(Customer entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
