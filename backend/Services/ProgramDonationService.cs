using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ProgramDonationService : IProgramDonationService
    {
        private readonly MyAppContext _context;

        public ProgramDonationService(MyAppContext context)
        {
            _context = context;
        }

        // Add Program Donation
        public async Task<ProgramDonation> AddProgramDonationAsync(ProgramDonation donation)
        {
            _context.ProgramDonations.Add(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        // Update Program Donation
        public async Task<ProgramDonation> UpdateProgramDonationAsync(int id, ProgramDonation updatedDonation)
        {
            var donation = await _context.ProgramDonations.FindAsync(id);
            if (donation == null) return null;

            donation.Amount = updatedDonation.Amount;
            donation.PaymentStatus = updatedDonation.PaymentStatus;
            donation.TargetAmount = updatedDonation.TargetAmount;  // Cập nhật số tiền mục tiêu
            donation.DonationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return donation;
        }

        // Delete Program Donation
        public async Task<bool> DeleteProgramDonationAsync(int id)
        {
            var donation = await _context.ProgramDonations.FindAsync(id);
            if (donation == null) return false;

            _context.ProgramDonations.Remove(donation);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get Program Donations with optional search query
        public async Task<List<ProgramDonation>> GetProgramDonationsAsync(string searchQuery = "")
        {
            var query = _context.ProgramDonations.Include(d => d.Program1).Include(d => d.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(d => d.PaymentStatus.Contains(searchQuery) || d.Program1.Name.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get Program Donation by Id
        public async Task<ProgramDonation?> GetProgramDonationByIdAsync(int id)
        {
            return await _context.ProgramDonations
                .Include(d => d.Program1)
                .Include(d => d.Customer)
                .FirstOrDefaultAsync(d => d.DonationId == id);
        }
    }

    public interface IProgramDonationService
    {
        Task<ProgramDonation> AddProgramDonationAsync(ProgramDonation donation);
        Task<ProgramDonation> UpdateProgramDonationAsync(int id, ProgramDonation updatedDonation);
        Task<bool> DeleteProgramDonationAsync(int id);
        Task<List<ProgramDonation>> GetProgramDonationsAsync(string searchQuery = "");
        Task<ProgramDonation?> GetProgramDonationByIdAsync(int id);
    }
}
