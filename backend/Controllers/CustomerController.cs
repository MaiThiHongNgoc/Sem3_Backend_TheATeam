using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        // Get all Customers (Admin only)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }


        // Add Customer (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required.");

            var addedCustomer = await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = addedCustomer.CustomerId }, addedCustomer);
        }

        // Update Customer (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            if (updatedCustomer == null)
                return BadRequest("Updated customer data is required.");

            var customer = await _customerService.UpdateCustomerAsync(id, updatedCustomer);
            if (customer == null)
                return NotFound("Customer not found.");

            return Ok(customer);
        }

        // Delete Customer (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
                return NotFound("Customer not found.");

            return NoContent();
        }

        // Get Customer by Id (Admin and User can access)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound("Customer not found.");

            return Ok(customer);
        }

        // Get Customers with optional search query (Admin and User can access)
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetCustomers([FromQuery] string searchQuery = "")
        {
            var customers = await _customerService.GetCustomersAsync(searchQuery);
            return Ok(customers);
        }
    }
}
