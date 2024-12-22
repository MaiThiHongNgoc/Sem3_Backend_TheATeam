using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class NGOService : INGOService
    {
        private readonly MyAppContext _context;

        public NGOService(MyAppContext context)
        {
            _context = context;
        }

        // Add NGO
        public async Task<NGO> AddNGOAsync(NGO ngo)
        {
            _context.NGOs.Add(ngo);
            await _context.SaveChangesAsync();
            return ngo;
        }

        // Update NGO
        public async Task<NGO> UpdateNGOAsync(int id, NGO updatedNGO)
        {
            var ngo = await _context.NGOs.FindAsync(id);
            if (ngo == null) return null;

            ngo.Name = updatedNGO.Name;
            ngo.Description = updatedNGO.Description;
            ngo.Code = updatedNGO.Code;
            ngo.LogoUrl = updatedNGO.LogoUrl;
            ngo.Mission = updatedNGO.Mission;
            ngo.Team = updatedNGO.Team;
            ngo.Careers = updatedNGO.Careers;
            ngo.Achievements = updatedNGO.Achievements;
            ngo.IsApproved = updatedNGO.IsApproved;
            ngo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ngo;
        }

        // Delete NGO
        public async Task<bool> DeleteNGOAsync(int id)
        {
            var ngo = await _context.NGOs.FindAsync(id);
            if (ngo == null) return false;

            _context.NGOs.Remove(ngo);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all NGOs with search functionality
        public async Task<List<NGO>> GetNGOsAsync(string searchQuery = "")
        {
            var query = _context.NGOs.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(n => n.Name.Contains(searchQuery) || n.Code.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get NGO by Id
        public async Task<NGO?> GetNGOByIdAsync(int id)
        {
            return await _context.NGOs
                .FirstOrDefaultAsync(n => n.NGOId == id);
        }
    }

    public interface INGOService
    {
        Task<NGO> AddNGOAsync(NGO ngo);
        Task<NGO> UpdateNGOAsync(int id, NGO updatedNGO);
        Task<bool> DeleteNGOAsync(int id);
        Task<List<NGO>> GetNGOsAsync(string searchQuery = "");
        Task<NGO?> GetNGOByIdAsync(int id);
    }
}
