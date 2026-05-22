using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Dto;
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
    public class GenresRepositoryTests
    {
        private Mock<BookShopContext> _mockContext;
        private GenresRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<BookShopContext>(new DbContextOptions<BookShopContext>());
            _repository = new GenresRepository(_mockContext.Object);
        }

        [Test]
        public async Task GetIdsAsync_ShouldReturnAllGenreIds()
        {
           
            var genres = new List<Genre>
            {
                new Genre { Id = Guid.NewGuid(), Name = "Genre 1" },
                new Genre { Id = Guid.NewGuid(), Name = "Genre 2" }
            };
            _mockContext.Setup(c => c.Genres).ReturnsDbSet(genres);

            
            var result = await _repository.GetIdsAsync();

           
            Assert.That(result.Count(), Is.EqualTo(genres.Count));
            Assert.That(result, Is.EquivalentTo(genres.Select(a => a.Id)));
        }

        [Test]
        public async Task GetIdsWithNamesAsync_ShouldReturnAllGenreIdsAndNames()
        {
           
            var genres = new List<Genre>
            {
                new Genre { Id = Guid.NewGuid(), Name = "Genre 1" },
                new Genre { Id = Guid.NewGuid(), Name = "Genre 2" }
            };
            _mockContext.Setup(c => c.Genres).ReturnsDbSet(genres);
            var expected = genres.Select(g => new IdWithName(g.Id, g.Name)).ToList();

           
            var result = await _repository.GetIdsWithNamesAsync();

           
            Assert.That(result.Count(), Is.EqualTo(expected.Count));
            Assert.That(result.First().Id, Is.EqualTo(expected.First().Id));
            Assert.That(result.First().Name, Is.EqualTo(expected.First().Name));
        }
    }
}
