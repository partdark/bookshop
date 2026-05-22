using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using Moq.EntityFrameworkCore;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshop.NUnitTests.Tests.Repositories
{
    public class BooksRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private BooksRepository _repository;
        private Mock<HybridCache> _mockCache;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<BookShopContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _mockContext = new Mock<BookShopContext>(options);
            _mockCache = new Mock<HybridCache>(MockBehavior.Loose);
            var dummyConnection = new NpgsqlConnection("Host=dummy;");
            _repository = new BooksRepository(_mockContext.Object, _mockCache.Object, dummyConnection);
        }

        [Test]
        public async Task TakeBookWithPagging_ShouldReturnPagedBooks()
        {
           
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Book 1" },
                new Book { Id = Guid.NewGuid(), Title = "Book 2" },
                new Book { Id = Guid.NewGuid(), Title = "Book 3" }
            };
            _mockContext.Setup(c => c.Books).ReturnsDbSet(books);

           
            var result = await _repository.TakeBookWithPagging(2, 1);

          
            Assert.That(result.Items.Count(), Is.EqualTo(2));
            Assert.That(result.TotalCount, Is.EqualTo(3));
            Assert.That(result.PageCount, Is.EqualTo(2));
            Assert.That(result.CurrentPage, Is.EqualTo(1));
            Assert.That(result.HasNext, Is.True);
            Assert.That(result.HasPrevious, Is.False);
        }
    }
}
