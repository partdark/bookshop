using Application.Dto;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookshop.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("/customers")]
        public async Task<ActionResult<List<CustomerResponseDto>>> GetAll()
        {
            return Ok(await _customerService.GetAll());
        }

        [HttpGet("/customer/{id:guid}")]
        public async Task<ActionResult<CustomerResponseDto>> GetById(Guid id)
        {
            var customer = await _customerService.GetById(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost("/customer/add")]
        public async Task<ActionResult<Guid>> Add([FromBody] AddCustomerDto customer)
        {
            var id = await _customerService.Add(customer);
            return Ok(id);
        }

        [HttpPut("/customer/update/{id:guid}")]
        public async Task<ActionResult<CustomerResponseDto>> Update(Guid id, [FromBody] UpdateCustomerDto customer)
        {
            var updatedCustomer = await _customerService.Update(id, customer);
            if (updatedCustomer == null)
            {
                return NotFound();
            }
            return Ok(updatedCustomer);
        }

        [HttpDelete("/customer/delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _customerService.Delete(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}
