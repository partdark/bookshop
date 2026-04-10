using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponseDto>> GetAll();
        Task<OrderResponseDto?> GetById(int id);
        Task<int> Add(AddOrderDto order);
        Task<bool> Delete(int id);
    }
}
