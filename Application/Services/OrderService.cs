using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IBooksRepository _booksRepository;
        private readonly HybridCache _cache;
       

        public OrderService(IOrdersRepository ordersRepository, IBooksRepository booksRepository,  HybridCache hybridCache)
        {
            _ordersRepository = ordersRepository;
            _booksRepository = booksRepository;
            _cache = hybridCache;
        
        }

        public async Task<int> Add(AddOrderDto orderDto)
        {
            using var transaction = await _ordersRepository.BeginTransactionAsync();
            try
            {
                 var orderItems = new List<OrderItems>();
            decimal totalPrice = 0;

        
            var books = new Dictionary<Guid, Book>();
            foreach (var itemDto in orderDto.Items)
            {
                var book = await _booksRepository.GetByIdAsync(itemDto.BookId);
                if (book == null)
                    throw new ArgumentException($"Книга с ID {itemDto.BookId} не найдена.");
                if (book.Count < itemDto.Count)
                    throw new ArgumentException($"Недостаточно экземпляров книги «{book.Title}»: доступно {book.Count}, запрошено {itemDto.Count}.");
                books[itemDto.BookId] = book;
            }

            foreach (var itemDto in orderDto.Items)
            {
                var book = books[itemDto.BookId];
                orderItems.Add(new OrderItems
                {
                    BookId = itemDto.BookId,
                    Count = itemDto.Count,
                    PriceAtPurchase = book.Price
                });
                totalPrice += book.Price * itemDto.Count;
                

            }

          
            foreach (var itemDto in orderDto.Items)
            {
                var newCount = books[itemDto.BookId].Count - itemDto.Count;
                await _booksRepository.UpdateCountAsync(itemDto.BookId, newCount);
            }

            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                CreatedDate = DateTime.UtcNow,
                TotalPrice = totalPrice,
                Status = OrderStatus.Placed,
                Items = orderItems
            };

            await _ordersRepository.AddAsync(order);
            await _cache.RemoveAsync("mainpage");
            return order.Id;
            }
            catch 
            {
                await transaction.RollbackAsync();
                throw;
            }
           
        }

        public async Task<bool> Delete(int id)
        {
            return await _ordersRepository.DeleteAsync(id);
        }

        public async Task<List<OrderResponseDto>> GetAll()
        {
            var orders = await _ordersRepository.GetAllAsync();
            return orders.Select(o => new OrderResponseDto(
                o.Id,
                o.CustomerId,
                o.CreatedDate,
                o.TotalPrice,
                o.Status.ToString(),
                o.Items.Select(oi => new OrderItemDto(oi.BookId, oi.Count, oi.PriceAtPurchase)).ToList()
            )).ToList();
        }

        public async Task<OrderResponseDto?> GetById(int id)
        {
            var order = await _ordersRepository.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }
            return new OrderResponseDto(
                order.Id,
                order.CustomerId,
                order.CreatedDate,
                order.TotalPrice,
                order.Status.ToString(),
                order.Items.Select(oi => new OrderItemDto(oi.BookId, oi.Count, oi.PriceAtPurchase)).ToList()
            );
        }
        public async Task<List<OrderResponseDto>> GetByCustomerId(Guid customerId)
        {
            var orders = await _ordersRepository.GetByCustomerIdAsync(customerId);
            return orders.Select(o => new OrderResponseDto(
                o.Id,
                o.CustomerId,
                o.CreatedDate,
                o.TotalPrice,
                o.Status.ToString(),
                o.Items?.Select(oi => new OrderItemDto(oi.BookId, oi.Count, oi.PriceAtPurchase)).ToList() ?? new()
            )).ToList();
        }

        public async Task<OrderDetailDto?> GetDetailedById(int id)
        {
            var order = await _ordersRepository.GetDetailedByIdAsync(id);
            if (order == null) return null;

            return new OrderDetailDto(
                order.Id,
                order.CustomerId,
                order.Customer?.UserName ?? string.Empty,
                order.Customer?.Email ?? string.Empty,
                order.CreatedDate,
                order.TotalPrice,
                order.Status.ToString(),
                order.Items?.Select(oi => new OrderItemDetailDto(
                    oi.BookId,
                    oi.Book?.Title ?? string.Empty,
                    oi.Book?.UrlImage ?? string.Empty,
                    oi.Count,
                    oi.PriceAtPurchase,
                    oi.PriceAtPurchase * oi.Count
                )).ToList() ?? new()
            );
        }

        public async Task<bool> UpdateStatus(int id, string status)
        {
            var order = await _ordersRepository.GetByIdAsync(id);
            if (order == null) return false;
            if (!Enum.TryParse<OrderStatus>(status, out var parsed)) return false;
            order.Status = parsed;
            await _ordersRepository.UpdateAsync(order);
            return true;
        }
    }
}
