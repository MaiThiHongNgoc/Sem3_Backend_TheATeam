using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TransactionHistoryService : ITransactionHistoryService
    {
        private readonly MyAppContext _context;

        public TransactionHistoryService(MyAppContext context)
        {
            _context = context;
        }

        // Add a new TransactionHistory
        public async Task<TransactionHistory> AddTransactionHistoryAsync(TransactionHistory transactionHistory)
        {
            _context.TransactionHistories.Add(transactionHistory);
            await _context.SaveChangesAsync();
            return transactionHistory;
        }

        // Update an existing TransactionHistory
        public async Task<TransactionHistory?> UpdateTransactionHistoryAsync(int id, TransactionHistory updatedTransactionHistory)
        {
            var transactionHistory = await _context.TransactionHistories.FindAsync(id);
            if (transactionHistory == null) return null;

            transactionHistory.TransactionDetails = updatedTransactionHistory.TransactionDetails;
            transactionHistory.TransactionDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return transactionHistory;
        }

        // Delete a TransactionHistory by Id
        public async Task<bool> DeleteTransactionHistoryAsync(int id)
        {
            var transactionHistory = await _context.TransactionHistories.FindAsync(id);
            if (transactionHistory == null) return false;

            _context.TransactionHistories.Remove(transactionHistory);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all TransactionHistories with optional search query
        public async Task<List<TransactionHistory>> GetTransactionHistoriesAsync(string searchQuery = "")
        {
            var query = _context.TransactionHistories.Include(t => t.Donation).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(t => t.TransactionDetails.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get a TransactionHistory by Id
        public async Task<TransactionHistory?> GetTransactionHistoryByIdAsync(int id)
        {
            return await _context.TransactionHistories
                .Include(t => t.Donation)
                .FirstOrDefaultAsync(t => t.TransactionId == id);
        }
    }

    public interface ITransactionHistoryService
    {
        Task<TransactionHistory> AddTransactionHistoryAsync(TransactionHistory transactionHistory);
        Task<TransactionHistory?> UpdateTransactionHistoryAsync(int id, TransactionHistory updatedTransactionHistory);
        Task<bool> DeleteTransactionHistoryAsync(int id);
        Task<List<TransactionHistory>> GetTransactionHistoriesAsync(string searchQuery = "");
        Task<TransactionHistory?> GetTransactionHistoryByIdAsync(int id);
    }
}
