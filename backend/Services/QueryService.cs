using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class QueryService : IQueryService
    {
        private readonly MyAppContext _context;

        public QueryService(MyAppContext context)
        {
            _context = context;
        }

        // Add a new Query
        public async Task<Query> AddQueryAsync(Query query)
        {
            _context.Queries.Add(query);
            await _context.SaveChangesAsync();
            return query;
        }

        // Update an existing Query
        public async Task<Query?> UpdateQueryAsync(int id, Query updatedQuery)
        {
            var query = await _context.Queries.FindAsync(id);
            if (query == null) return null;

            query.ReplyText = updatedQuery.ReplyText;
            query.Status = updatedQuery.Status;

            await _context.SaveChangesAsync();
            return query;
        }

        // Delete a Query by Id
        public async Task<bool> DeleteQueryAsync(int id)
        {
            var query = await _context.Queries.FindAsync(id);
            if (query == null) return false;

            _context.Queries.Remove(query);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all Queries with optional search query
        public async Task<List<Query>> GetQueriesAsync(string searchQuery = "")
        {
            var query = _context.Queries.Include(q => q.Customer).Include(q => q.Program1).AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(q => q.Subject.Contains(searchQuery) || q.QueryText.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }

        // Get a Query by Id
        public async Task<Query?> GetQueryByIdAsync(int id)
        {
            return await _context.Queries
                .Include(q => q.Customer)
                .Include(q => q.Program1)
                .FirstOrDefaultAsync(q => q.QueryId == id);
        }

        // Get all Queries for a specific customer (if required)
        public async Task<List<Query>> GetQueriesByCustomerIdAsync(int customerId)
        {
            return await _context.Queries
                .Include(q => q.Customer)
                .Include(q => q.Program1)
                .Where(q => q.CustomerId == customerId)
                .ToListAsync();
        }
    }

    public interface IQueryService
    {
        Task<Query> AddQueryAsync(Query query);
        Task<Query?> UpdateQueryAsync(int id, Query updatedQuery);
        Task<bool> DeleteQueryAsync(int id);
        Task<List<Query>> GetQueriesAsync(string searchQuery = "");
        Task<Query?> GetQueryByIdAsync(int id);
        Task<List<Query>> GetQueriesByCustomerIdAsync(int customerId);
    }
}
