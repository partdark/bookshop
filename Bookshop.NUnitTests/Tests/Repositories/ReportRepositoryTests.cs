using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookshop.NUnitTests.Tests.Repositories;

[TestFixture]
public class ReportRepositoryTests
{
    private ReportRepository _repository;
    private BookShopContext _context;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BookShopContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new BookShopContext(options);
        _repository = new ReportRepository(_context);
    }

    [TearDown]
    public void Teardown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task OrderCountAsync_ReturnsEmptyList_WhenNoOrders()
    {
      
        var result = await _repository.OrderCountAsync();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task OrderCountAsync_ReturnsCorrectCount_WhenOrdersExist()
    {
        
        var orders = new List<Order>
        {
            new() { Status = OrderStatus.Placed, CreatedDate = DateTime.UtcNow },
            new() { Status = OrderStatus.Placed, CreatedDate = DateTime.UtcNow },
            new() { Status = OrderStatus.Shipped, CreatedDate = DateTime.UtcNow }
        };
        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();

       
        var result = await _repository.OrderCountAsync();

       
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(r => r.Name == "Placed" && r.Count == 2));
        Assert.That(result.Any(r => r.Name == "Shipped" && r.Count == 1));
    }

    [Test]
    public async Task OrdersMoneyAsync_ReturnsEmptyList_WhenNoOrders()
    {
       
        var result = await _repository.OrdersMoneyAsync();

      
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task OrdersMoneyAsync_ReturnsCorrectMoney_WhenOrdersExist()
    {
       
        var orders = new List<Order>
        {
            new() { Status = OrderStatus.Placed, TotalPrice = 100m, CreatedDate = DateTime.UtcNow },
            new() { Status = OrderStatus.Placed, TotalPrice = 200m, CreatedDate = DateTime.UtcNow },
            new() { Status = OrderStatus.Shipped, TotalPrice = 150m, CreatedDate = DateTime.UtcNow }
        };
        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();

      
        var result = await _repository.OrdersMoneyAsync();

      
        Assert.That(result.Count, Is.EqualTo(2));
        var placedResult = result.FirstOrDefault(r => r.Name == "Placed");
        Assert.That(placedResult, Is.Not.Null);
        Assert.That(placedResult.Count, Is.EqualTo(2));
        Assert.That(placedResult.TotalMoney, Is.EqualTo(300m));
        
        var shippedResult = result.FirstOrDefault(r => r.Name == "Shipped");
        Assert.That(shippedResult, Is.Not.Null);
        Assert.That(shippedResult.Count, Is.EqualTo(1));
        Assert.That(shippedResult.TotalMoney, Is.EqualTo(150m));
    }
}
