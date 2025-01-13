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

        // Thêm mới donation
        public async Task<ProgramDonation> AddProgramDonationAsync(ProgramDonation donation)
        {
            _context.ProgramDonations.Add(donation);
            await _context.SaveChangesAsync();
            return donation;
        }

        // Lấy danh sách donation theo NGO và chương trình
        public async Task<List<DonationDTO>> GetDonationsForProgramAndNGOAsync(int ngoId, int programId)
        {
            var donations = await _context.ProgramDonations
                .Include(d => d.Program1)
                .Include(d => d.Customer)
                .ThenInclude(c => c.Account)
                .Where(d => d.Program1!.NGOId == ngoId && d.ProgramId == programId)
                .Select(d => new DonationDTO
                {
                    DonationId = d.DonationId,
                    Amount = d.Amount,
                    PaymentStatus = d.PaymentStatus,
                    DonationDate = d.DonationDate,
                    DonorName = $"{d.Customer!.FirstName} {d.Customer.LastName}",
                    DonorEmail = d.Customer.Account!.Email,
                    ProgramName = d.Program1!.Name
                })
                .ToListAsync();

            return donations;
        }

        // Cập nhật donation
        public async Task<ProgramDonation> UpdateProgramDonationAsync(int id, ProgramDonation updatedDonation)
        {
            var donation = await _context.ProgramDonations.FindAsync(id);
            if (donation == null) return null;

            donation.Amount = updatedDonation.Amount;
            donation.PaymentStatus = updatedDonation.PaymentStatus;
            donation.DonationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return donation;
        }

        // Xóa donation
        public async Task<bool> DeleteProgramDonationAsync(int id)
        {
            var donation = await _context.ProgramDonations.FindAsync(id);
            if (donation == null) return false;

            _context.ProgramDonations.Remove(donation);
            await _context.SaveChangesAsync();
            return true;
        }

        // Lấy tất cả donation (tùy chọn tìm kiếm)
        public async Task<List<ProgramDonation>> GetProgramDonationsAsync(string searchQuery = "")
        {
            var query = _context.ProgramDonations.Include(d => d.Program1).Include(d => d.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(d => d.PaymentStatus.Contains(searchQuery) || d.Program1.Name.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Lấy donation theo ID
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
        Task<List<DonationDTO>> GetDonationsForProgramAndNGOAsync(int ngoId, int programId);
    }
}
