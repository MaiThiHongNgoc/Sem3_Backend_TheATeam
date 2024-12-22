using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryImageController : ControllerBase
    {
        private readonly IGalleryImageService _galleryImageService;

        public GalleryImageController(IGalleryImageService galleryImageService)
        {
            _galleryImageService = galleryImageService;
        }

        // Get all GalleryImages with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetGalleryImages([FromQuery] string searchQuery = "")
        {
            var galleryImages = await _galleryImageService.GetGalleryImagesAsync(searchQuery);
            return Ok(galleryImages);
        }

        // Get GalleryImage by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetGalleryImageById(int id)
        {
            var galleryImage = await _galleryImageService.GetGalleryImageByIdAsync(id);
            if (galleryImage == null)
                return NotFound("Gallery Image not found.");

            return Ok(galleryImage);
        }

        // Add GalleryImage (Admin, NGO roles)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddGalleryImage([FromBody] GalleryImage galleryImage)
        {
            if (galleryImage == null)
                return BadRequest("Gallery Image data is required.");

            var addedGalleryImage = await _galleryImageService.AddGalleryImageAsync(galleryImage);
            return CreatedAtAction(nameof(GetGalleryImageById), new { id = addedGalleryImage.ImageId }, addedGalleryImage);
        }

        // Update GalleryImage (Admin, NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateGalleryImage(int id, [FromBody] GalleryImage updatedGalleryImage)
        {
            if (updatedGalleryImage == null)
                return BadRequest("Updated Gallery Image data is required.");

            var galleryImage = await _galleryImageService.UpdateGalleryImageAsync(id, updatedGalleryImage);
            if (galleryImage == null)
                return NotFound("Gallery Image not found.");

            return Ok(galleryImage);
        }

        // Delete GalleryImage (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteGalleryImage(int id)
        {
            var result = await _galleryImageService.DeleteGalleryImageAsync(id);
            if (!result)
                return NotFound("Gallery Image not found.");

            return NoContent();
        }
    }
}
