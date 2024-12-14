using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryController : ControllerBase {
        private readonly MyAppContext _context;

        public GalleryController(MyAppContext  context) {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllImages() {
            var images = await _context.Galleries.ToListAsync();
            return Ok(images);
        }

        [HttpPost]
        [Authorize(Roles = "NGO")]
        public async Task<IActionResult> AddImage([FromBody] Gallery gallery) {
            gallery.CreatedAt = DateTime.UtcNow;
            _context.Galleries.Add(gallery);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetImageById), new { id = gallery.ImageId }, gallery);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetImageById(int id) {
            var image = await _context.Galleries.FindAsync(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "NGO")]
        public async Task<IActionResult> UpdateImage(int id, [FromBody] Gallery updatedGallery) {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery == null) return NotFound();
            gallery.ImageUrl = updatedGallery.ImageUrl;
            gallery.Caption = updatedGallery.Caption;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "NGO")]
        public async Task<IActionResult> DeleteImage(int id) {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery == null) return NotFound();
            _context.Galleries.Remove(gallery);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
