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

        /// <summary>
        /// Get all NGOs with optional search query.
        /// </summary>
        /// <param name="searchQuery">Search string to filter NGOs by Name or Code.</param>
        /// <returns>List of NGOs.</returns>
        [HttpGet]
        public async Task<IActionResult> GetNGOs([FromQuery] string searchQuery = "")
        {
            var ngos = await _ngoService.GetNGOsAsync(searchQuery);
            return Ok(ngos);
        }

        /// <summary>
        /// Get a single NGO by its ID.
        /// </summary>
        /// <param name="id">The ID of the NGO.</param>
        /// <returns>The NGO data or Not Found status.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNGOById(int id)
        {
            var ngo = await _ngoService.GetNGOByIdAsync(id);
            if (ngo == null)
                return NotFound("NGO not found.");

            return Ok(ngo);
        }

        /// <summary>
        /// Add a new NGO.
        /// </summary>
        /// <param name="ngo">NGO object to create.</param>
        /// <returns>Created NGO or Bad Request.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddNGO([FromBody] NGO ngo)
        {
            if (ngo == null || string.IsNullOrWhiteSpace(ngo.Email))
                return BadRequest("Valid NGO data with an email is required.");

            try
            {
                var addedNGO = await _ngoService.AddNGOAsync(ngo);
                return CreatedAtAction(nameof(GetNGOById), new { id = addedNGO.NGOId }, addedNGO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing NGO.
        /// </summary>
        /// <param name="id">The ID of the NGO to update.</param>
        /// <param name="updatedNGO">Updated NGO data.</param>
        /// <returns>The updated NGO or Not Found status.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateNGO(int id, [FromBody] NGO updatedNGO)
        {
            if (updatedNGO == null)
                return BadRequest("Updated NGO data is required.");

            try
            {
                var ngo = await _ngoService.UpdateNGOAsync(id, updatedNGO);
                if (ngo == null)
                    return NotFound("NGO not found.");

                return Ok(ngo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete an NGO by its ID.
        /// </summary>
        /// <param name="id">The ID of the NGO to delete.</param>
        /// <returns>No Content status or Not Found.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNGO(int id)
        {
            var result = await _ngoService.DeleteNGOAsync(id);
            if (!result)
                return NotFound("NGO not found.");

            return NoContent();
        }

        /// <summary>
        /// Approve or disapprove an NGO.
        /// </summary>
        /// <param name="id">The ID of the NGO.</param>
        /// <param name="isApproved">Approval status.</param>
        /// <returns>Updated NGO or Not Found status.</returns>
        [HttpPatch("{id}/approval")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveNGO(int id, [FromQuery] bool isApproved)
        {
            var ngo = await _ngoService.GetNGOByIdAsync(id);
            if (ngo == null)
                return NotFound("NGO not found.");

            ngo.IsApproved = isApproved;
            await _ngoService.UpdateNGOAsync(id, ngo);
            return Ok(ngo);
        }

        /// <summary>
        /// Search NGOs by multiple filters.
        /// </summary>
        /// <param name="name">Filter by name.</param>
        /// <param name="code">Filter by code.</param>
        /// <param name="isApproved">Filter by approval status.</param>
        /// <returns>List of filtered NGOs.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchNGOs([FromQuery] string? name, [FromQuery] string? code, [FromQuery] bool? isApproved)
        {
            var ngos = await _ngoService.SearchNGOsAsync(name, code, isApproved);
            return Ok(ngos);
        }
    }
}
