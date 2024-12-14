using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramRegistrationController : ControllerBase {
        private readonly MyAppContext _context;

        public ProgramRegistrationController(MyAppContext context) {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllRegistrations() {
            var registrations = await _context.ProgramRegistrations
                                              .Include(r => r.Program1)
                                              .Include(r => r.Customer)
                                              .ToListAsync();
            return Ok(registrations);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetRegistrationById(int id) {
            var registration = await _context.ProgramRegistrations
                                             .Include(r => r.Program1)
                                             .Include(r => r.Customer)
                                             .FirstOrDefaultAsync(r => r.RegistrationId == id);

            if (registration == null) return NotFound();
            return Ok(registration);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddRegistration([FromBody] ProgramRegistration registration) {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            registration.RegisteredAt = DateTime.UtcNow;
            _context.ProgramRegistrations.Add(registration);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRegistrationById), new { id = registration.RegistrationId }, registration);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> DeleteRegistration(int id) {
            var registration = await _context.ProgramRegistrations.FindAsync(id);
            if (registration == null) return NotFound();

            _context.ProgramRegistrations.Remove(registration);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
