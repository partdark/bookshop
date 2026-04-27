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
    public class AuthorsRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private AuthorsRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _repository = new AuthorsRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetIdsAsync_ShouldReturnAllAuthorIds()
        {
            
            var authors = new List<Author>
            {
                new Author { Id = Guid.NewGuid(), Name = "Author 1" },
                new Author { Id = Guid.NewGuid(), Name = "Author 2" }
            };
            _mockContext.Setup(c => c.Authors).ReturnsDbSet(authors);

           
            var result = await _repository.GetIdsAsync();

            
            Assert.That(result.Count(), Is.EqualTo(authors.Count));
            Assert.That(result, Is.EquivalentTo(authors.Select(a => a.Id)));
        }
    }
}
