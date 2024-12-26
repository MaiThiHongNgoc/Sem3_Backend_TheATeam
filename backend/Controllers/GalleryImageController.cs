using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GalleryImageController : ControllerBase
    {
        private readonly IGalleryImageService _galleryImageService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GalleryImageController(IGalleryImageService galleryImageService, IWebHostEnvironment webHostEnvironment)
        {
            _galleryImageService = galleryImageService;
            _webHostEnvironment = webHostEnvironment;
        }

        // Get all GalleryImages with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        public async Task<IActionResult> GetGalleryImages([FromQuery] string searchQuery = "")
        {
            var galleryImages = await _galleryImageService.GetGalleryImagesAsync(searchQuery);
            return Ok(galleryImages);
        }

        // Get GalleryImage by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGalleryImageById(int id)
        {
            var galleryImage = await _galleryImageService.GetGalleryImageByIdAsync(id);
            if (galleryImage == null)
                return NotFound("Gallery Image not found.");

            // Generate the image URL for the client
            var imageUrl = Path.Combine("/images", galleryImage.FileName);
            galleryImage.FileName = imageUrl;

            return Ok(galleryImage);
        }

        // Add GalleryImage (Admin, NGO roles)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddGalleryImage([FromForm] IFormFile imageFile, [FromForm] string caption)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No image file provided.");

            // Generate a unique file name to avoid conflicts
            var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

            // Save the image to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Create the GalleryImage object and save it to the database
            var galleryImage = new GalleryImage
            {
                FileName = fileName,
                Caption = caption,
                CreatedAt = DateTime.UtcNow
            };

            var addedGalleryImage = await _galleryImageService.AddGalleryImageAsync(galleryImage);
            return CreatedAtAction(nameof(GetGalleryImageById), new { id = addedGalleryImage.ImageId }, addedGalleryImage);
        }


        // Update GalleryImage (Admin, NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateGalleryImage(int id, [FromForm] IFormFile? imageFile, [FromForm] string caption)
        {
            var updatedGalleryImage = new GalleryImage
            {
                Caption = caption,
                CreatedAt = DateTime.UtcNow
            };

            if (imageFile != null && imageFile.Length > 0)
            {
                // Generate a new file name
                var fileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                // Save the new image file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                updatedGalleryImage.FileName = fileName;
            }

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
            var galleryImage = await _galleryImageService.GetGalleryImageByIdAsync(id);
            if (galleryImage == null)
                return NotFound("Gallery Image not found.");

            // Delete the image file from the server
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", galleryImage.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var result = await _galleryImageService.DeleteGalleryImageAsync(id);
            if (!result)
                return NotFound("Gallery Image not found.");

            return NoContent();
        }
    }
}
