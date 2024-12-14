using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/ngos")]
    public class NgoController : ControllerBase
    {
        private readonly MyAppContext _context;

        public NgoController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả NGOs
        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllNgos()
        {
            var ngos = await _context.NGOs.ToListAsync();
            return Ok(ngos);
        }

        // Lấy thông tin NGO theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetNgoById(int id)
        {
            var ngo = await _context.NGOs.FindAsync(id);
            if (ngo == null)
                return NotFound("NGO not found.");

            return Ok(ngo);
        }

        // Thêm mới một NGO
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNgo([FromBody] NGO request)
        {
            _context.NGOs.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetNgoById), new { id = request.NGOId }, request);
        }

        // Cập nhật thông tin NGO
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateNgo(int id, [FromBody] NGO request)
        {
            var ngo = await _context.NGOs.FindAsync(id);
            if (ngo == null)
                return NotFound("NGO not found.");

            ngo.Name = request.Name;
            ngo.Description = request.Description;
            ngo.LogoUrl = request.LogoUrl;
            ngo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("NGO updated successfully.");
        }

        // Xóa một NGO
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNgo(int id)
        {
            var ngo = await _context.NGOs.FindAsync(id);
            if (ngo == null)
                return NotFound("NGO not found.");

            _context.NGOs.Remove(ngo);
            await _context.SaveChangesAsync();
            return Ok("NGO deleted successfully.");
        }
    }
}
