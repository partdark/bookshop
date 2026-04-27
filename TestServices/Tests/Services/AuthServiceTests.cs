
using Application.Dto;
using Application.Interfaces;
using Application.Services;
using Castle.Core.Configuration;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Bookshop.Tests.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<Customer>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly IAuthService _authService;

        public AuthServiceTests()
        {
            var userStoreMock = new Mock<IUserStore<Customer>>();
            _mockUserManager = new Mock<UserManager<Customer>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();

            _authService = new AuthService(_mockUserManager.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnAuthResponseDto_WhenRegistrationIsSuccessful()
        {
            
            var registerDto = new RegisterDto("Test User", "test@example.com", "password", "1234567890", DateOnly.FromDateTime(DateTime.UtcNow));

            _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email)).ReturnsAsync((Customer)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<Customer>(), registerDto.Password)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<Customer>(), "user")).ReturnsAsync(IdentityResult.Success);

            var configSectionJwtKey = new Mock<IConfigurationSection>();
            configSectionJwtKey.Setup(s => s.Value).Returns("your-super-secret-key-that-is-long-enough");
            _mockConfiguration.Setup(c => c.GetSection("Jwt:Key")).Returns(configSectionJwtKey.Object);

            var configSectionJwtExpires = new Mock<IConfigurationSection>();
            configSectionJwtExpires.Setup(s => s.Value).Returns("60");
            _mockConfiguration.Setup(c => c.GetSection("Jwt:ExpiresInMinutes")).Returns(configSectionJwtExpires.Object);
            
            var configSectionJwtIssuer = new Mock<IConfigurationSection>();
            configSectionJwtIssuer.Setup(s => s.Value).Returns("your-issuer");
            _mockConfiguration.Setup(c => c.GetSection("Jwt:Issuer")).Returns(configSectionJwtIssuer.Object);
            
            var configSectionJwtAudience = new Mock<IConfigurationSection>();
            configSectionJwtAudience.Setup(s => s.Value).Returns("your-audience");
            _mockConfiguration.Setup(c => c.GetSection("Jwt:Audience")).Returns(configSectionJwtAudience.Object);
            
            _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<Customer>())).ReturnsAsync(["user"]);

            _mockUserManager.Setup(um => um.UpdateAsync(It.IsAny<Customer>())).ReturnsAsync(IdentityResult.Success);


           
            var result = await _authService.Register(registerDto);

          
            Assert.NotNull(result);
            Assert.IsType<AuthResponseDto>(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("user", result.Role);
            Assert.Equal(registerDto.Name, result.Customer.Name);
            Assert.Equal(registerDto.Email, result.Customer.Mail);
        }
    }
}
