using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NGOController : ControllerBase
    {
        private readonly INGOService _ngoService;

        public NGOController(INGOService ngoService)
        {
            _ngoService = ngoService;
        }

        // Get all NGOs with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetNGOs([FromQuery] string searchQuery = "")
        {
            var ngos = await _ngoService.GetNGOsAsync(searchQuery);
            return Ok(ngos);
        }

        // Get NGO by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetNGOById(int id)
        {
            var ngo = await _ngoService.GetNGOByIdAsync(id);
            if (ngo == null)
                return NotFound("NGO not found.");

            return Ok(ngo);
        }

        // Add NGO (Admin role only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNGO([FromBody] NGO ngo)
        {
            if (ngo == null)
                return BadRequest("NGO data is required.");

            var addedNGO = await _ngoService.AddNGOAsync(ngo);
            return CreatedAtAction(nameof(GetNGOById), new { id = addedNGO.NGOId }, addedNGO);
        }

        // Update NGO (Admin and NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateNGO(int id, [FromBody] NGO updatedNGO)
        {
            if (updatedNGO == null)
                return BadRequest("Updated NGO data is required.");

            var ngo = await _ngoService.UpdateNGOAsync(id, updatedNGO);
            if (ngo == null)
                return NotFound("NGO not found.");

            return Ok(ngo);
        }

        // Delete NGO (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNGO(int id)
        {
            var result = await _ngoService.DeleteNGOAsync(id);
            if (!result)
                return NotFound("NGO not found.");

            return NoContent();
        }
    }
}
