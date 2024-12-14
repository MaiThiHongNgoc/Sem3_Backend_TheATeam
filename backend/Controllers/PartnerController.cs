using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/partners")]
    public class PartnerController : ControllerBase
    {
        private readonly MyAppContext _context;

        public PartnerController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả Partners
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPartners()
        {
            var partners = await _context.Partners.Include(p => p.Account).ToListAsync();
            return Ok(partners);
        }

        // Lấy thông tin Partner theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Partner")]
        public async Task<IActionResult> GetPartnerById(int id)
        {
            var partner = await _context.Partners.Include(p => p.Account).FirstOrDefaultAsync(p => p.PartnerId == id);
            if (partner == null)
                return NotFound("Partner not found.");

            // Kiểm tra quyền truy cập
            if (User.IsInRole("Admin") || partner.Account.Email == User.Identity.Name)
            {
                return Ok(partner);
            }

            return Unauthorized("Bạn không có quyền truy cập.");
        }

        // Thêm mới một Partner
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePartner([FromBody] Partner request)
        {
            _context.Partners.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPartnerById), new { id = request.PartnerId }, request);
        }

        // Cập nhật thông tin Partner
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Partner")]
        public async Task<IActionResult> UpdatePartner(int id, [FromBody] Partner request)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
                return NotFound("Partner not found.");

            partner.CompanyName = request.CompanyName;
            partner.BankAccount = request.BankAccount;
            partner.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Partner updated successfully.");
        }

        // Xóa một Partner
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
                return NotFound("Partner not found.");

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
            return Ok("Partner deleted successfully.");
        }
    }

}