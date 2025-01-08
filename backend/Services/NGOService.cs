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

        // Add NGO with Account
        public async Task<NGO> AddNGOAsync(NGO ngo)
        {
            if (string.IsNullOrWhiteSpace(ngo.Email))
                throw new ArgumentException("Email is required.");

            // Use EF.Functions.Like for case-insensitive email comparison
            if (await _context.Accounts.AnyAsync(a => EF.Functions.Like(a.Email, ngo.Email)))
                throw new InvalidOperationException("Email is already in use.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Fetch the role for NGOs (assuming RoleId 3 corresponds to NGOs)
                var role = await _context.Roles.FindAsync(3);
                if (role == null)
                    throw new InvalidOperationException("Role not found.");

                // Create an account with the fetched Role
                var account = new Account
                {
                    Email = ngo.Email,
                    Username = ngo.Code,
                    Password = BCrypt.Net.BCrypt.HashPassword("DefaultPassword123"), // Ensure this method is correct
                    IsActive = true,
                    Role = role,
                    Customer = null
                };

                // Add the account and save changes to get the generated AccountId
                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                // Set the AccountId in the NGO object
                ngo.AccountId = account.AccountId;

                // Add the NGO and save changes
                _context.NGOs.Add(ngo);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return ngo;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        // Update NGO
        public async Task<NGO> UpdateNGOAsync(int id, NGO updatedNGO)
        {
            var ngo = await _context.NGOs.Include(n => n.Account).FirstOrDefaultAsync(n => n.NGOId == id);
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
            ngo.Email = updatedNGO.Email; // Update email

            // Update Account email if changed
            if (!string.Equals(ngo.Account.Email, updatedNGO.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (_context.Accounts.Any(a => a.Email == updatedNGO.Email))
                    throw new InvalidOperationException("Email is already in use.");

                ngo.Account.Email = updatedNGO.Email; // Update Account email
            }

            ngo.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return ngo;
        }

        // Delete NGO
        public async Task<bool> DeleteNGOAsync(int id)
        {
            var ngo = await _context.NGOs.Include(n => n.Account).FirstOrDefaultAsync(n => n.NGOId == id);
            if (ngo == null) return false;

            _context.Accounts.Remove(ngo.Account); // Remove associated account
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
            return await _context.NGOs.Include(n => n.Account).FirstOrDefaultAsync(n => n.NGOId == id);
        }

        // Search NGOs with filters
        public async Task<List<NGO>> SearchNGOsAsync(string? name, string? code, bool? isApproved)
        {
            var query = _context.NGOs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(n => n.Name.Contains(name));

            if (!string.IsNullOrWhiteSpace(code))
                query = query.Where(n => n.Code.Contains(code));

            if (isApproved.HasValue)
                query = query.Where(n => n.IsApproved == isApproved.Value);

            return await query.ToListAsync();
        }

        private string HashPassword(string password)
        {
            // Implement a secure password hashing algorithm (e.g., BCrypt or SHA256)
            return password; // Placeholder
        }
    }

    public interface INGOService
    {
        Task<NGO> AddNGOAsync(NGO ngo);
        Task<NGO> UpdateNGOAsync(int id, NGO updatedNGO);
        Task<bool> DeleteNGOAsync(int id);
        Task<List<NGO>> GetNGOsAsync(string searchQuery = "");
        Task<NGO?> GetNGOByIdAsync(int id);
        Task<List<NGO>> SearchNGOsAsync(string? name, string? code, bool? isApproved);
    }
}
