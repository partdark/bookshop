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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomersRepository _customersRepository;

        public CustomerService(ICustomersRepository customersRepository)
        {
            _customersRepository = customersRepository;
        }

        public async Task<Guid> Add(AddCustomerDto customerDto)
        {
            var customer = new Customer
            {
                Name = customerDto.Name,
                Mail = customerDto.Mail,
                Phone = customerDto.Phone,
                DateOfBirth = customerDto.DateOfBirth,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(customerDto.Password)
            };
            await _customersRepository.AddAsync(customer);
            return customer.Id;
        }

        public async Task<bool> Delete(Guid id)
        {
            return await _customersRepository.DeleteAsync(id);
        }

        public async Task<List<CustomerResponseIdNameDto>> GetAll()
        {
            var customers = await _customersRepository.GetIdsWithNamesAsync();
            return customers.Select(c => new CustomerResponseIdNameDto(c.Id, c.Name)).ToList();
        }

        public async Task<CustomerResponseDto?> GetById(Guid id)
        {
            var customer = await _customersRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            return new CustomerResponseDto(customer.Id, customer.Name, customer.Mail, customer.Phone, customer.DateOfBirth);
        }

        public async Task<CustomerResponseDto?> Update(Guid id, UpdateCustomerDto customerDto)
        {
            var customer = await _customersRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }

            customer.Name = customerDto.Name;
            customer.Mail = customerDto.Mail;
            customer.Phone = customerDto.Phone;
            customer.DateOfBirth = customerDto.DateOfBirth;
            
            var updatedCustomer = await _customersRepository.UpdateAsync(customer);
            if(updatedCustomer == null)
            {
                return null;
            }
            return new CustomerResponseDto(customer.Id, customer.Name, customer.Mail, customer.Phone, customer.DateOfBirth);
        }

       
    }
}
