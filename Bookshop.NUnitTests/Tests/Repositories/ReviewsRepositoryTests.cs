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
    public class ReviewsRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private ReviewsRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _repository = new ReviewsRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetAll_ShouldReturnAllReviews()
        {
           
            var reviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), Rating = 5 },
                new Review { Id = Guid.NewGuid(), Rating = 4 }
            };
            _mockContext.Setup(c => c.Reviews).ReturnsDbSet(reviews);

           
            var result = await _repository.GetAll();

           
            Assert.That(result.Count(), Is.EqualTo(reviews.Count));
        }
    }
}
