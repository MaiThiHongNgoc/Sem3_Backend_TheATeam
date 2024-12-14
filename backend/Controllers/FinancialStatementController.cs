using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialStatementController : ControllerBase {
        private readonly MyAppContext _context;

        public FinancialStatementController(MyAppContext context) {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllStatements() {
            var statements = await _context.FinancialStatements
                                           .Include(s => s.NGO)
                                           .Include(s => s.Donation)
                                           .ToListAsync();
            return Ok(statements);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetStatementById(int id) {
            var statement = await _context.FinancialStatements
                                          .Include(s => s.NGO)
                                          .Include(s => s.Donation)
                                          .FirstOrDefaultAsync(s => s.StatementId == id);

            if (statement == null) return NotFound();
            return Ok(statement);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddStatement([FromBody] FinancialStatement statement) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            statement.StatementDate = DateTime.UtcNow;
            _context.FinancialStatements.Add(statement);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStatementById), new { id = statement.StatementId }, statement);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateStatement(int id, [FromBody] FinancialStatement updatedStatement) {
            var statement = await _context.FinancialStatements.FindAsync(id);
            if (statement == null) return NotFound();

            statement.Amount = updatedStatement.Amount;
            statement.Note = updatedStatement.Note;
            statement.StatementDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> DeleteStatement(int id) {
            var statement = await _context.FinancialStatements.FindAsync(id);
            if (statement == null) return NotFound();

            _context.FinancialStatements.Remove(statement);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
