using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/donations")]
    public class DonationController : ControllerBase
    {
        private readonly MyAppContext _context;

        public DonationController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả các khoản đóng góp
        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllDonations()
        {
            var donations = await _context.Donations.Include(d => d.Customer).Include(d => d.Cause).Include(d => d.NGO).ToListAsync();
            return Ok(donations);
        }

        // Lấy thông tin đóng góp theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetDonationById(int id)
        {
            var donation = await _context.Donations.Include(d => d.Customer).Include(d => d.Cause).Include(d => d.NGO).FirstOrDefaultAsync(d => d.DonationId == id);
            if (donation == null)
                return NotFound("Donation not found.");

            return Ok(donation);
        }

        // Thêm khoản đóng góp mới
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateDonation([FromBody] Donation request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Donations.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDonationById), new { id = request.DonationId }, request);
        }

        // Cập nhật thông tin đóng góp
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateDonation(int id, [FromBody] Donation request)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
                return NotFound("Donation not found.");

            donation.Amount = request.Amount;
            donation.PaymentStatus = request.PaymentStatus;
            donation.DistributionStatus = request.DistributionStatus;

            await _context.SaveChangesAsync();
            return Ok("Donation updated successfully.");
        }

        // Xóa khoản đóng góp
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null)
                return NotFound("Donation not found.");

            _context.Donations.Remove(donation);
            await _context.SaveChangesAsync();
            return Ok("Donation deleted successfully.");
        }
    }
}
