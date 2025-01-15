using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Program1Controller : ControllerBase
    {
        private readonly IProgram1Service _program1Service;

        public Program1Controller(IProgram1Service program1Service)
        {
            _program1Service = program1Service;
        }

        // Get all Program1s with optional search query and pagination (Admin, User, and NGO roles)
        [HttpGet]
        public async Task<IActionResult> GetProgram1s([FromQuery] string searchQuery = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var programs = await _program1Service.GetProgram1sAsync(searchQuery, page, pageSize);
            return Ok(programs);
        }

        // Get Program1 by Id (Admin, User, and NGO roles)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgram1ById(int id)
        {
            var program = await _program1Service.GetProgram1ByIdAsync(id);
            if (program == null)
                return NotFound("Program not found.");

            return Ok(program);
        }

        // Add Program1 (Admin role only)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddProgram1([FromBody] Program1 program)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (program == null)
                    return BadRequest("Program data is required.");

                var addedProgram = await _program1Service.AddProgram1Async(program);
                return CreatedAtAction(nameof(GetProgram1ById), new { id = addedProgram.ProgramId }, addedProgram);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // Update Program1 (Admin and NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateProgram1(int id, [FromBody] Program1 updatedProgram)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (updatedProgram == null)
                    return BadRequest("Updated Program data is required.");

                var program = await _program1Service.UpdateProgram1Async(id, updatedProgram);
                if (program == null)
                    return NotFound("Program not found.");

                return Ok(program);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        // Delete Program1 (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProgram1(int id)
        {
            var result = await _program1Service.DeleteProgram1Async(id);
            if (!result)
                return NotFound("Program not found.");

            return NoContent();
        }
    }
}
