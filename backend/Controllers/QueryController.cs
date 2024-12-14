using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase {
        private readonly MyAppContext _context;

        public QueryController(MyAppContext  context) {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetAllQueries() {
            var queries = await _context.Queries.ToListAsync();
            return Ok(queries);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetQueryById(int id) {
            var query = await _context.Queries.FindAsync(id);
            if (query == null) return NotFound();
            return Ok(query);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddQuery([FromBody] Query query) {
            query.CreatedAt = DateTime.UtcNow;
            query.UpdatedAt = DateTime.UtcNow;
            _context.Queries.Add(query);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQueryById), new { id = query.QueryId }, query);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReplyToQuery(int id, [FromBody] string reply) {
            var query = await _context.Queries.FindAsync(id);
            if (query == null) return NotFound();
            query.Reply = reply;
            query.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuery(int id) {
            var query = await _context.Queries.FindAsync(id);
            if (query == null) return NotFound();
            _context.Queries.Remove(query);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
