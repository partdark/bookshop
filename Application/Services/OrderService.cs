using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Dto;
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


        public OrderService(IOrdersRepository ordersRepository, IBooksRepository booksRepository, HybridCache hybridCache)
        {
            _ordersRepository = ordersRepository;
            _booksRepository = booksRepository;
            _cache = hybridCache;

        }

        public async Task<int> Add(AddOrderDto orderDto)
        {
          return  await _ordersRepository.CreateAsync(orderDto);
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