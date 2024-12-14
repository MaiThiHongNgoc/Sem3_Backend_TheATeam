using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly MyAppContext _context;

        public CategoryController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả các danh mục
        [HttpGet]
        [AllowAnonymous] // Public API
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // Lấy danh mục theo ID
        [HttpGet("{id}")]
        [AllowAnonymous] // Public API
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            return Ok(category);
        }

        // Tìm kiếm danh mục theo tên
        [HttpGet("search")]
        [AllowAnonymous] // Public API
        public async Task<IActionResult> SearchCategories([FromQuery] string keyword)
        {
            var categories = await _context.Categories
                .Where(c => c.CategoryName.Contains(keyword))
                .ToListAsync();

            if (categories.Count == 0)
                return NotFound("No matching categories found.");

            return Ok(categories);
        }

        // Tạo danh mục mới
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] Category request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Categories.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCategoryById), new { id = request.CategoryId }, request);
        }

        // Cập nhật danh mục
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            category.CategoryName = request.CategoryName;
            category.Description = request.Description;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Category updated successfully.");
        }

        // Xóa danh mục
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok("Category deleted successfully.");
        }
    }
}
