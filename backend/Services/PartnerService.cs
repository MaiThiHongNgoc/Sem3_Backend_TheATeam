using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PartnerService : IPartnerService
    {
        private readonly MyAppContext _context;

        public PartnerService(MyAppContext context)
        {
            _context = context;
        }

        // Add Partner
        public async Task<Partner> AddPartnerAsync(Partner partner)
        {
            _context.Partners.Add(partner);
            await _context.SaveChangesAsync();
            return partner;
        }

        // Update Partner
        public async Task<Partner> UpdatePartnerAsync(int id, Partner updatedPartner)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null) return null;

            partner.Name = updatedPartner.Name;
            partner.Description = updatedPartner.Description;
            partner.LogoUrl = updatedPartner.LogoUrl;
            partner.IsApproved = updatedPartner.IsApproved;
            partner.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return partner;
        }

        // Delete Partner
        public async Task<bool> DeletePartnerAsync(int id)
        {
            var partner = await _context.Partners.FindAsync(id);
            if (partner == null) return false;

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all Partners with optional search query
        public async Task<List<Partner>> GetPartnersAsync(string searchQuery = "")
        {
            var query = _context.Partners.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get Partner by Id
        public async Task<Partner?> GetPartnerByIdAsync(int id)
        {
            return await _context.Partners
                .FirstOrDefaultAsync(p => p.PartnerId == id);
        }
    }

    public interface IPartnerService
    {
        Task<Partner> AddPartnerAsync(Partner partner);
        Task<Partner> UpdatePartnerAsync(int id, Partner updatedPartner);
        Task<bool> DeletePartnerAsync(int id);
        Task<List<Partner>> GetPartnersAsync(string searchQuery = "");
        Task<Partner?> GetPartnerByIdAsync(int id);
    }
}
