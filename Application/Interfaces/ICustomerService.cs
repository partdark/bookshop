using Application.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerResponseIdNameDto>> GetAll();
        Task<CustomerResponseDto?> GetById(Guid id);
        Task<Guid> Add(AddCustomerDto customer);
        Task<CustomerResponseDto?> Update(Guid id, UpdateCustomerDto customer);
        Task<bool> Delete(Guid id);
    }
}
