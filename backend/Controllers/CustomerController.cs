using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("get-customer-data")]
        public IActionResult GetCustomerData()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return Unauthorized();

            var accountIdClaim = identity.FindFirst("id");
            if (accountIdClaim == null)
            {
                Console.WriteLine("Token không chứa claim 'id'.");
                return NotFound("AccountId không tồn tại trong token.");
            }

            if (!int.TryParse(accountIdClaim.Value, out var accountId))
            {
                Console.WriteLine("AccountId trong token không hợp lệ.");
                return BadRequest("AccountId không hợp lệ.");
            }

            var customer = _customerService.GetCustomerByAccountId(accountId);
            if (customer == null)
            {
                Console.WriteLine($"Không tìm thấy khách hàng với AccountId: {accountId}");
                return NotFound("Không tìm thấy thông tin khách hàng.");
            }

            Console.WriteLine($"Tìm thấy khách hàng: {customer.CustomerId}");
            return Ok(customer);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            if (customer == null)
                return BadRequest("Customer data is required.");

            var addedCustomer = await _customerService.AddCustomerAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = addedCustomer.CustomerId }, addedCustomer);
        }

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result)
                return NotFound("Customer not found.");

            return NoContent();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound("Customer not found.");

            return Ok(customer);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetCustomers([FromQuery] string searchQuery = "")
        {
            var customers = await _customerService.GetCustomersAsync(searchQuery);
            return Ok(customers);
        }
    }
}
