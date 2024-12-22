using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        // Get all Invitations with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetInvitations([FromQuery] string searchQuery = "")
        {
            var invitations = await _invitationService.GetInvitationsAsync(searchQuery);
            return Ok(invitations);
        }

        // Get Invitation by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,User,NGO")]
        public async Task<IActionResult> GetInvitationById(int id)
        {
            var invitation = await _invitationService.GetInvitationByIdAsync(id);
            if (invitation == null)
                return NotFound("Invitation not found.");

            return Ok(invitation);
        }

        // Add Invitation (Admin, NGO roles)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddInvitation([FromBody] Invitation invitation)
        {
            if (invitation == null)
                return BadRequest("Invitation data is required.");

            var addedInvitation = await _invitationService.AddInvitationAsync(invitation);
            return CreatedAtAction(nameof(GetInvitationById), new { id = addedInvitation.InvitationId }, addedInvitation);
        }

        // Update Invitation (Admin, NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateInvitation(int id, [FromBody] Invitation updatedInvitation)
        {
            if (updatedInvitation == null)
                return BadRequest("Updated invitation data is required.");

            var invitation = await _invitationService.UpdateInvitationAsync(id, updatedInvitation);
            if (invitation == null)
                return NotFound("Invitation not found.");

            return Ok(invitation);
        }

        // Delete Invitation (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInvitation(int id)
        {
            var result = await _invitationService.DeleteInvitationAsync(id);
            if (!result)
                return NotFound("Invitation not found.");

            return NoContent();
        }
    }
}
