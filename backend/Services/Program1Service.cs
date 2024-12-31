using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class Program1Service : IProgram1Service
    {
        private readonly MyAppContext _context;

        public Program1Service(MyAppContext context)
        {
            _context = context;
        }

        // Add Program1
        public async Task<Program1> AddProgram1Async(Program1 program)
        {
            var ngoExists = await _context.NGOs.AnyAsync(n => n.NGOId == program.NGOId);
            if (!ngoExists)
                throw new InvalidOperationException("The specified NGO does not exist.");

            _context.Program1s.Add(program);
            await _context.SaveChangesAsync();
            return program;
        }

        // Update Program1
        public async Task<Program1> UpdateProgram1Async(int id, Program1 updatedProgram)
        {
            var program = await _context.Program1s.FindAsync(id);
            if (program == null)
                return null;

            var ngoExists = await _context.NGOs.AnyAsync(n => n.NGOId == updatedProgram.NGOId);
            if (!ngoExists)
                throw new InvalidOperationException("The specified NGO does not exist.");

            program.Name = updatedProgram.Name;
            program.Description = updatedProgram.Description;
            program.StartDate = updatedProgram.StartDate;
            program.EndDate = updatedProgram.EndDate;
            program.IsUpcoming = updatedProgram.IsUpcoming;
            program.TargetAmount = updatedProgram.TargetAmount;
            program.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return program;
        }

        // Delete Program1
        public async Task<bool> DeleteProgram1Async(int id)
        {
            var program = await _context.Program1s.FindAsync(id);
            if (program == null) return false;

            _context.Program1s.Remove(program);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all Program1s with optional search query and pagination
        public async Task<List<Program1>> GetProgram1sAsync(string searchQuery = "", int page = 1, int pageSize = 10)
        {
            // var query = _context.Program1s.Include(p => p.NGO).AsQueryable();

            // if (!string.IsNullOrEmpty(searchQuery))
            // {
            //     query = query.Where(p => p.Name.Contains(searchQuery) || p.Description.Contains(searchQuery));
            // }
            

            return await _context.Program1s.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // Get Program1 by Id
        public async Task<Program1?> GetProgram1ByIdAsync(int id)
        {
            return await _context.Program1s
                .Include(p => p.NGO)
                .Include(p => p.Donations)
                .Include(p => p.GalleryImages)
                .FirstOrDefaultAsync(p => p.ProgramId == id);
        }
    }

    public interface IProgram1Service
    {
        Task<Program1> AddProgram1Async(Program1 program);
        Task<Program1> UpdateProgram1Async(int id, Program1 updatedProgram);
        Task<bool> DeleteProgram1Async(int id);
        Task<List<Program1>> GetProgram1sAsync(string searchQuery = "", int page = 1, int pageSize = 10);
        Task<Program1?> GetProgram1ByIdAsync(int id);
    }
}
