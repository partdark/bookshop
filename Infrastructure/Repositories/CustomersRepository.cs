using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<Customer> AddAsync(Customer entity, string password)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existing = await _userManager.FindByEmailAsync(entity.Email!);
            if (existing != null)
                throw new InvalidOperationException($"Customer with email {entity.Email} already exists");

            // UserManager.CreateAsync сам хэширует пароль через PBKDF2
            var result = await _userManager.CreateAsync(entity, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(entity, "user");
            return entity;
        }

        
        async Task<Customer?> IRepository<Customer>.AddAsync(Customer entity)
            => await AddAsync(entity, string.Empty);

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
          
            var tracked = await _userManager.FindByIdAsync(entity.Id.ToString());
            if (tracked == null) throw new InvalidOperationException("Customer not found");

            tracked.UserName = entity.UserName;
            tracked.NormalizedUserName = entity.UserName?.ToUpper();
            tracked.Email = entity.Email;
            tracked.NormalizedEmail = entity.Email?.ToUpper();
            tracked.PhoneNumber = entity.PhoneNumber;
            tracked.DateOfBirth = entity.DateOfBirth;
            tracked.RefreshToken = entity.RefreshToken;
            tracked.RefreshTokenExpiry = entity.RefreshTokenExpiry;

            await _userManager.UpdateAsync(tracked);
            return tracked;
        }
    }
}
