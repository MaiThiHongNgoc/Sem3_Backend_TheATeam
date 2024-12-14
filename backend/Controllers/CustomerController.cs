using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly MyAppContext _context;

        public CustomerController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả khách hàng
        [HttpGet]
        [Authorize(Roles = "Admin")]  // Chỉ Admin có quyền truy cập
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _context.Customers.Include(c => c.Account).ToListAsync();
            return Ok(customers);
        }

        // Lấy thông tin khách hàng theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User")]  // Admin và User có thể xem thông tin của chính mình
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _context.Customers.Include(c => c.Account).FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return NotFound("Customer not found.");

            // Kiểm tra nếu người dùng yêu cầu thông tin của chính mình hoặc Admin truy cập
            if (User.IsInRole("Admin") || customer.Account.Email == User.Identity.Name)
            {
                return Ok(customer);
            }

            return Unauthorized("Bạn không có quyền truy cập.");
        }
        // Thêm mới một khách hàng
        [HttpPost]
        [Authorize(Roles = "Admin")]  // Chỉ Admin có quyền thêm khách hàng mới
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
        {
            // Kiểm tra nếu tài khoản đã tồn tại
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email);
            if (existingAccount != null)
            {
                return BadRequest("Email đã được sử dụng.");
            }

            // Tạo tài khoản mới
            var account = new Account
            {
                Email = request.Email,
                Password = request.Password, // Mã hóa mật khẩu trước khi lưu
                RoleId = request.RoleId
            };

            // Tạo khách hàng mới
            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Account = account // Liên kết với tài khoản mới
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customer);
        }


        // Cập nhật thông tin khách hàng
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]  // Admin và User có thể cập nhật thông tin của chính mình
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return NotFound("Customer not found.");

            // Kiểm tra nếu người dùng yêu cầu cập nhật thông tin của chính mình hoặc Admin
            if (User.IsInRole("Admin") || customer.Account.Email == User.Identity.Name)
            {
                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;
                customer.DateOfBirth = request.DateOfBirth;

                await _context.SaveChangesAsync();
                return Ok("Customer updated successfully.");
            }

            return Unauthorized("Bạn không có quyền sửa đổi thông tin này.");
        }

        // Xóa khách hàng
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Chỉ Admin có quyền xóa khách hàng
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
                return NotFound("Customer not found.");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return Ok("Customer deleted successfully.");
        }
    }
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; } // Vai trò của tài khoản (Admin, User, NGO, ...)
    }


}