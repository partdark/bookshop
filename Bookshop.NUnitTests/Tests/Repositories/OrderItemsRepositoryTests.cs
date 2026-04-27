using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshop.NUnitTests.Tests.Repositories
{
    public class OrderItemsRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private OrderItemsRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _repository = new OrderItemsRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetByOrderIdAsync_ShouldReturnOrderItems()
        {
          
            var orderId = 1;
            var orderItems = new List<OrderItems>
            {
                new OrderItems { OrderId = orderId, BookId = Guid.NewGuid() },
                new OrderItems { OrderId = orderId, BookId = Guid.NewGuid() }
            };
            _mockContext.Setup(c => c.OrderItems).ReturnsDbSet(orderItems);

            
            var result = await _repository.GetByOrderIdAsync(orderId);

            Assert.That(result.Count(), Is.EqualTo(orderItems.Count));
        }
    }
}
