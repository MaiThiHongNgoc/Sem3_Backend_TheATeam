using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly MyAppContext _context;

        public PaymentController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả thanh toán
        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.Payments.Include(p => p.Donation).ToListAsync();
            return Ok(payments);
        }

        // Lấy thanh toán theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _context.Payments.Include(p => p.Donation).FirstOrDefaultAsync(p => p.PaymentId == id);
            if (payment == null)
                return NotFound("Payment not found.");

            return Ok(payment);
        }

        // Thêm thanh toán mới
        [HttpPost]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> CreatePayment([FromBody] Payment request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Payments.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPaymentById), new { id = request.PaymentId }, request);
        }

        // Xóa thanh toán
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound("Payment not found.");

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return Ok("Payment deleted successfully.");
        }
    }
}
