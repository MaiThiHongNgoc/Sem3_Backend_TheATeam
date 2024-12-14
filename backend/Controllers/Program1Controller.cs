using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class Program1Controller : ControllerBase {
        private readonly MyAppContext _context;

        public Program1Controller(MyAppContext context) {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllPrograms() {
            var programs = await _context.Program1s.ToListAsync();
            return Ok(programs);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetProgramById(int id) {
            var program = await _context.Program1s.FindAsync(id);
            if (program == null) return NotFound();
            return Ok(program);
        }

        [HttpPost]
        [Authorize(Roles = "NGO")]
        public async Task<IActionResult> AddProgram([FromBody] Program1 program) {
            program.CreatedAt = DateTime.UtcNow;
            program.UpdatedAt = DateTime.UtcNow;
            _context.Program1s.Add(program);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProgramById), new { id = program.ProgramId }, program);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "NGO")]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] Program1 updatedProgram) {
            var program = await _context.Program1s.FindAsync(id);
            if (program == null) return NotFound();
            program.Title = updatedProgram.Title;
            program.Description = updatedProgram.Description;
            program.StartDate = updatedProgram.StartDate;
            program.EndDate = updatedProgram.EndDate;
            program.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "NGO")]
        public async Task<IActionResult> DeleteProgram(int id) {
            var program = await _context.Program1s.FindAsync(id);
            if (program == null) return NotFound();
            _context.Program1s.Remove(program);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
