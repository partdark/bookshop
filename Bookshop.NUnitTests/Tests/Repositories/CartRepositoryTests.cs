using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshop.NUnitTests.Tests.Repositories
{
    public class CartRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private CartRepository _repository;
        private Mock<HybridCache> _mockCache;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _mockCache = new Mock<HybridCache>(MockBehavior.Loose);
            _repository = new CartRepository(_mockContext.Object, _mockCache.Object);
        }

        [Test]
        public async Task GetCartItemsByCustomerId_ShouldReturnCartItems()
        {
            
            var customerId = Guid.NewGuid();
            var customers = new List<Customer> { new Customer { Id = customerId, UserName = "Test Customer" } };
            var cartItems = new List<CartItem>
            {
                new CartItem { Id = 1, CustomerId = customerId },
                new CartItem { Id = 1, CustomerId = customerId }
            };
            _mockContext.Setup(c => c.Users).ReturnsDbSet(customers);
            _mockContext.Setup(c => c.CartItems).ReturnsDbSet(cartItems);

            
            var result = await _repository.GetCartItemsByCustomerId(customerId);

           
            Assert.That(result.Count(), Is.EqualTo(cartItems.Count));
        }
    }
}
