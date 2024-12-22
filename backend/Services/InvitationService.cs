using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly MyAppContext _context;

        public InvitationService(MyAppContext context)
        {
            _context = context;
        }

        // Add Invitation
        public async Task<Invitation> AddInvitationAsync(Invitation invitation)
        {
            _context.Invitations.Add(invitation);
            await _context.SaveChangesAsync();
            return invitation;
        }

        // Update Invitation
        public async Task<Invitation> UpdateInvitationAsync(int id, Invitation updatedInvitation)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation == null) return null;

            invitation.RecipientEmail = updatedInvitation.RecipientEmail;
            invitation.Message = updatedInvitation.Message;
            invitation.Status = updatedInvitation.Status;
            invitation.SentAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return invitation;
        }

        // Delete Invitation
        public async Task<bool> DeleteInvitationAsync(int id)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation == null) return false;

            _context.Invitations.Remove(invitation);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get Invitations with optional search query
        public async Task<List<Invitation>> GetInvitationsAsync(string searchQuery = "")
        {
            var query = _context.Invitations.Include(i => i.Sender).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(i => i.RecipientEmail.Contains(searchQuery) || i.Sender.FirstName.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get Invitation by Id
        public async Task<Invitation?> GetInvitationByIdAsync(int id)
        {
            return await _context.Invitations
                .Include(i => i.Sender)
                .FirstOrDefaultAsync(i => i.InvitationId == id);
        }
    }

    public interface IInvitationService
    {
        Task<Invitation> AddInvitationAsync(Invitation invitation);
        Task<Invitation> UpdateInvitationAsync(int id, Invitation updatedInvitation);
        Task<bool> DeleteInvitationAsync(int id);
        Task<List<Invitation>> GetInvitationsAsync(string searchQuery = "");
        Task<Invitation?> GetInvitationByIdAsync(int id);
    }
}
