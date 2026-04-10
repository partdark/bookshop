using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Interfaces;
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

        public OrderService(IOrdersRepository ordersRepository, IBooksRepository booksRepository)
        {
            _ordersRepository = ordersRepository;
            _booksRepository = booksRepository;
        }

        public async Task<int> Add(AddOrderDto orderDto)
        {
            var orderItems = new List<OrderItems>();
            decimal totalPrice = 0;

            foreach (var itemDto in orderDto.Items)
            {
                var book = await _booksRepository.GetByIdAsync(itemDto.BookId);
                if (book == null)
                {
                    throw new ArgumentException($"Book with ID {itemDto.BookId} not found.");
                }
                orderItems.Add(new OrderItems
                {
                    BookId = itemDto.BookId,
                    Book = book, 
                    Count = itemDto.Count,
                    PriceAtPurchase = book.Price 
                });
                totalPrice += book.Price * itemDto.Count;
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
            return order.Id;
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
    }
}
