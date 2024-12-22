using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class GalleryImageService : IGalleryImageService
    {
        private readonly MyAppContext _context;

        public GalleryImageService(MyAppContext context)
        {
            _context = context;
        }

        // Add GalleryImage
        public async Task<GalleryImage> AddGalleryImageAsync(GalleryImage galleryImage)
        {
            _context.GalleryImages.Add(galleryImage);
            await _context.SaveChangesAsync();
            return galleryImage;
        }

        // Update GalleryImage
        public async Task<GalleryImage?> UpdateGalleryImageAsync(int id, GalleryImage updatedGalleryImage)
        {
            var galleryImage = await _context.GalleryImages.FindAsync(id);
            if (galleryImage == null) return null;

            galleryImage.ImageUrl = updatedGalleryImage.ImageUrl;
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
            var query = _context.GalleryImages.Include(g => g.Program1).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(g => g.ImageUrl.Contains(searchQuery) || g.Caption.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get GalleryImage by Id
        public async Task<GalleryImage?> GetGalleryImageByIdAsync(int id)
        {
            return await _context.GalleryImages
                .Include(g => g.Program1)
                .FirstOrDefaultAsync(g => g.ImageId == id);
        }
    }

    public interface IGalleryImageService
    {
        Task<GalleryImage> AddGalleryImageAsync(GalleryImage galleryImage);
        Task<GalleryImage?> UpdateGalleryImageAsync(int id, GalleryImage updatedGalleryImage);
        Task<bool> DeleteGalleryImageAsync(int id);
        Task<List<GalleryImage>> GetGalleryImagesAsync(string searchQuery = "");
        Task<GalleryImage?> GetGalleryImageByIdAsync(int id);
    }
}
