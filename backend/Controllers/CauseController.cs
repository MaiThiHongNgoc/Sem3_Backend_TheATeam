using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/causes")]
    public class CauseController : ControllerBase
    {
        private readonly MyAppContext _context;

        public CauseController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả các nguyên nhân
        [HttpGet]
        [AllowAnonymous] // Public API
        public async Task<IActionResult> GetAllCauses()
        {
            var causes = await _context.Causes.Include(c => c.Category).ToListAsync();
            return Ok(causes);
        }

        // Lấy nguyên nhân theo ID
        [HttpGet("{id}")]
        [AllowAnonymous] // Public API
        public async Task<IActionResult> GetCauseById(int id)
        {
            var cause = await _context.Causes.Include(c => c.Category).FirstOrDefaultAsync(c => c.CauseId == id);
            if (cause == null)
                return NotFound("Cause not found.");

            return Ok(cause);
        }

        // Tìm kiếm nguyên nhân theo tên
        [HttpGet("search")]
        [AllowAnonymous] // Public API
        public async Task<IActionResult> SearchCauses([FromQuery] string keyword)
        {
            var causes = await _context.Causes
                .Include(c => c.Category)
                .Where(c => c.CauseName.Contains(keyword))
                .ToListAsync();

            if (causes.Count == 0)
                return NotFound("No matching causes found.");

            return Ok(causes);
        }

        // Tạo nguyên nhân mới
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> CreateCause([FromBody] Cause request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
                return BadRequest("Invalid category ID.");

            _context.Causes.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCauseById), new { id = request.CauseId }, request);
        }

        // Cập nhật nguyên nhân
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateCause(int id, [FromBody] Cause request)
        {
            var cause = await _context.Causes.FindAsync(id);
            if (cause == null)
                return NotFound("Cause not found.");

            cause.CauseName = request.CauseName;
            cause.Description = request.Description;
            cause.CategoryId = request.CategoryId;
            cause.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Cause updated successfully.");
        }

        // Xóa nguyên nhân
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCause(int id)
        {
            var cause = await _context.Causes.FindAsync(id);
            if (cause == null)
                return NotFound("Cause not found.");

            _context.Causes.Remove(cause);
            await _context.SaveChangesAsync();
            return Ok("Cause deleted successfully.");
        }
    }
}
