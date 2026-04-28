using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
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
    public class CustomersRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private CustomersRepository _repository;
        private Mock<UserManager<Customer>> _mockUserManager;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<Customer>>();
            _mockUserManager = new Mock<UserManager<Customer>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _repository = new CustomersRepository(_mockContext.Object, _mockUserManager.Object);
        }

        [Test]
        public async Task GetIdsAsync_ShouldReturnAllCustomerIds()
        {
         
            var customers = new List<Customer>
            {
                new Customer { Id = Guid.NewGuid(), UserName = "Customer 1" },
                new Customer { Id = Guid.NewGuid(), UserName = "Customer 2" }
            };
            _mockContext.Setup(c => c.Users).ReturnsDbSet(customers);

           
            var result = await _repository.GetIdsAsync();

          
            Assert.That(result.Count(), Is.EqualTo(customers.Count));
            Assert.That(result, Is.EquivalentTo(customers.Select(a => a.Id)));
        }

        [Test]
        public async Task GetIdsWithNamesAsync_ShouldReturnAllCustomerIdsAndNames()
        {
          
            var customers = new List<Customer>
            {
                new() { Id = Guid.NewGuid(), UserName = "Customer 1" },
                new() { Id = Guid.NewGuid(), UserName = "Customer 2" }
            };
            _mockContext.Setup(c => c.Users).ReturnsDbSet(customers);
            var expected = customers.Select(g => new IdWithNAme(g.Id, g.UserName)).ToList();

            
            var result = await _repository.GetIdsWithNamesAsync();

           
            Assert.That(result.Count, Is.EqualTo(expected.Count));
            Assert.That(result.First().Id, Is.EqualTo(expected.First().Id));
            Assert.That(result.First().Name, Is.EqualTo(expected.First().Name));
        }
    }
}
