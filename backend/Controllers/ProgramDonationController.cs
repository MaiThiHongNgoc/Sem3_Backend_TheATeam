using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramDonationController : ControllerBase
    {
        private readonly IProgramDonationService _programDonationService;

        public ProgramDonationController(IProgramDonationService programDonationService)
        {
            _programDonationService = programDonationService;
        }

        // Get all Program Donations with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetProgramDonations([FromQuery] string searchQuery = "")
        {
            var donations = await _programDonationService.GetProgramDonationsAsync(searchQuery);
            return Ok(donations);
        }

        // Get Program Donation by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetProgramDonationById(int id)
        {
            var donation = await _programDonationService.GetProgramDonationByIdAsync(id);
            if (donation == null)
                return NotFound("Donation not found.");

            return Ok(donation);
        }

        // Add Program Donation (Admin and NGO roles)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddProgramDonation([FromBody] ProgramDonation donation)
        {
            if (donation == null)
                return BadRequest("Donation data is required.");

            var addedDonation = await _programDonationService.AddProgramDonationAsync(donation);
            return CreatedAtAction(nameof(GetProgramDonationById), new { id = addedDonation.DonationId }, addedDonation);
        }

        // Update Program Donation (Admin and NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateProgramDonation(int id, [FromBody] ProgramDonation updatedDonation)
        {
            if (updatedDonation == null)
                return BadRequest("Updated donation data is required.");

            var donation = await _programDonationService.UpdateProgramDonationAsync(id, updatedDonation);
            if (donation == null)
                return NotFound("Donation not found.");

            return Ok(donation);
        }

        // Delete Program Donation (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProgramDonation(int id)
        {
            var result = await _programDonationService.DeleteProgramDonationAsync(id);
            if (!result)
                return NotFound("Donation not found.");

            return NoContent();
        }
    }
}
