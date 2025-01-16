using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetProgramDonationsAsync([FromQuery] string searchQuery = "")
        {
            var result = await _programDonationService.GetProgramDonationsAsync(searchQuery);
            return Ok(result);
        }
        [HttpGet("ngo/{ngoId}/program/{programId}")]
        public async Task<IActionResult> GetDonationsForProgramAndNGO(int ngoId, int programId)
        {
            var donations = await _programDonationService.GetDonationsForProgramAndNGOAsync(ngoId, programId);
            return Ok(donations);
        }

        // Get Program Donation by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgramDonationById(int id)
        {
            var donation = await _programDonationService.GetProgramDonationByIdAsync(id);
            if (donation == null)
                return NotFound("Donation not found.");

            return Ok(new
            {
                donation.DonationId,
                donation.Amount,
                donation.PaymentStatus,
                donation.DonationDate,
                Program = donation.Program1.Name,
                Customer = donation.Customer.FirstName + " " + donation.Customer.LastName
            });
        }

        // Add Program Donation (Admin and NGO roles)
        [HttpPost]
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
