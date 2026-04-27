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
    public class OrdersRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private OrdersRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _repository = new OrdersRepository(_mockContext.Object, null);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
           
            var orders = new List<Order>
            {
                new Order { Id = 1, CreatedDate = DateTime.Now },
                new Order { Id = 2, CreatedDate = DateTime.Now }
            };
            _mockContext.Setup(c => c.Orders).ReturnsDbSet(orders);

          
            var result = await _repository.GetAllAsync();

           
            Assert.That(result.Count(), Is.EqualTo(orders.Count));
        }
    }
}
