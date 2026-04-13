using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Customer> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<Customer> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> Register(RegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null) return null;

            var customer = new Customer
            {
                UserName = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                NormalizedEmail = dto.Email.ToUpper(),
                NormalizedUserName = dto.Name.ToUpper()
            };

            var result = await _userManager.CreateAsync(customer, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(customer, "user");
            return await BuildResponse(customer);
        }

        public async Task<AuthResponseDto?> RegisterAdmin(RegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null) return null;

            var customer = new Customer
            {
                UserName = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                NormalizedEmail = dto.Email.ToUpper(),
                NormalizedUserName = dto.Name.ToUpper()
            };

            var result = await _userManager.CreateAsync(customer, dto.Password);
            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(customer, "Admin");
            return await BuildResponse(customer);
        }

        public async Task<AuthResponseDto?> Login(LoginDto dto)
        {
            var customer = await _userManager.FindByEmailAsync(dto.Email);
            if (customer == null) return null;
            if (!await _userManager.CheckPasswordAsync(customer, dto.Password)) return null;
            return await BuildResponse(customer);
        }

        public async Task<AuthResponseDto?> Refresh(string refreshToken)
        {
            var customer = _userManager.Users
                .FirstOrDefault(u => u.RefreshToken == refreshToken
                                  && u.RefreshTokenExpiry > DateTime.UtcNow);
            if (customer == null) return null;
            return await BuildResponse(customer);
        }

        public async Task<bool> ChangePassword(Guid customerId, ChangePasswordDto dto)
        {
            var customer = await _userManager.FindByIdAsync(customerId.ToString());
            if (customer == null) return false;
            var result = await _userManager.ChangePasswordAsync(customer, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task RevokeToken(Guid customerId)
        {
            var customer = await _userManager.FindByIdAsync(customerId.ToString());
            if (customer == null) return;
            customer.RefreshToken = null;
            customer.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(customer);
        }

        // ── helpers ──────────────────────────────────────────────────────────

        private async Task<AuthResponseDto> BuildResponse(Customer customer)
        {
            var roles = await _userManager.GetRolesAsync(customer);
            var role = roles.Contains("Admin") ? "Admin" : "user";
            var accessToken = await GenerateAccessToken(customer);
            var refreshToken = GenerateRefreshToken();

            customer.RefreshToken = refreshToken;
            customer.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
            await _userManager.UpdateAsync(customer);

            var dto = new CustomerResponseDto(
                customer.Id, customer.UserName!, customer.Email!,
                customer.PhoneNumber ?? "", customer.DateOfBirth);

            return new AuthResponseDto(accessToken, refreshToken, dto, role);
        }

        private async Task<string> GenerateAccessToken(Customer customer)
        {
            var roles = await _userManager.GetRolesAsync(customer);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresInMinutes"] ?? "60"));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, customer.Email!),
                new Claim(ClaimTypes.Name, customer.UserName!),
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
