using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        private readonly IPartnerService _partnerService;

        public PartnerController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }

        // Get all Partners with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        public async Task<IActionResult> GetPartners([FromQuery] string searchQuery = "")
        {
            var partners = await _partnerService.GetPartnersAsync(searchQuery);
            return Ok(partners);
        }

        // Get Partner by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPartnerById(int id)
        {
            var partner = await _partnerService.GetPartnerByIdAsync(id);
            if (partner == null)
                return NotFound("Partner not found.");

            return Ok(partner);
        }

        // Add Partner (Admin role only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPartner([FromBody] Partner partner)
        {
            if (partner == null)
                return BadRequest("Partner data is required.");

            var addedPartner = await _partnerService.AddPartnerAsync(partner);
            return CreatedAtAction(nameof(GetPartnerById), new { id = addedPartner.PartnerId }, addedPartner);
        }

        // Update Partner (Admin and NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdatePartner(int id, [FromBody] Partner updatedPartner)
        {
            if (updatedPartner == null)
                return BadRequest("Updated Partner data is required.");

            var partner = await _partnerService.UpdatePartnerAsync(id, updatedPartner);
            if (partner == null)
                return NotFound("Partner not found.");

            return Ok(partner);
        }

        // Delete Partner (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePartner(int id)
        {
            var result = await _partnerService.DeletePartnerAsync(id);
            if (!result)
                return NotFound("Partner not found.");

            return NoContent();
        }
    }
}
