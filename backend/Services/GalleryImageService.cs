using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace backend.Services
{
    public class GalleryImageService : IGalleryImageService
    {
        private readonly MyAppContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Inject IWebHostEnvironment
        public GalleryImageService(MyAppContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Add GalleryImage
        public async Task<GalleryImage> AddGalleryImageAsync(IFormFile imageFile, string? caption, int programId)
        {
            // Kiểm tra ProgramId có tồn tại không
            var programExists = await _context.Program1s.AnyAsync(p => p.ProgramId == programId);
            if (!programExists)
            {
                throw new ArgumentException("Invalid ProgramId. Program does not exist.");
            }

            // Tạo tên file duy nhất
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, fileName);

            // Lưu file lên server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Tạo đối tượng GalleryImage
            var galleryImage = new GalleryImage
            {
                FileName = fileName,
                Caption = caption,
                ProgramId = programId,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu đối tượng vào cơ sở dữ liệu
            _context.GalleryImages.Add(galleryImage);
            await _context.SaveChangesAsync();

            return galleryImage;
        }

        // Update GalleryImage
        public async Task<GalleryImage?> UpdateGalleryImageAsync(int id, GalleryImage updatedGalleryImage)
        {
            var galleryImage = await _context.GalleryImages.FindAsync(id);
            if (galleryImage == null) return null;

            galleryImage.FileName = updatedGalleryImage.FileName;
            galleryImage.Caption = updatedGalleryImage.Caption;
            galleryImage.CreatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return galleryImage;
        }

        // Delete GalleryImage
        public async Task<bool> DeleteGalleryImageAsync(int id)
        {
            var galleryImage = await _context.GalleryImages.FindAsync(id);
            if (galleryImage == null) return false;

            _context.GalleryImages.Remove(galleryImage);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all GalleryImages with optional search query
        public async Task<List<GalleryImage>> GetGalleryImagesAsync(string searchQuery = "")
        {
            var query = _context.GalleryImages.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(g => g.FileName.Contains(searchQuery) || g.Caption.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get GalleryImage by Id
        public async Task<GalleryImage?> GetGalleryImageByIdAsync(int id)
        {
            return await _context.GalleryImages.FindAsync(id);
        }
    }

    public interface IGalleryImageService
    {
        Task<GalleryImage> AddGalleryImageAsync(IFormFile imageFile, string? caption, int programId);
        Task<GalleryImage?> UpdateGalleryImageAsync(int id, GalleryImage updatedGalleryImage);
        Task<bool> DeleteGalleryImageAsync(int id);
        Task<List<GalleryImage>> GetGalleryImagesAsync(string searchQuery = "");
        Task<GalleryImage?> GetGalleryImageByIdAsync(int id);
    }
}
