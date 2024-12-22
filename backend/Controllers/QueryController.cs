using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly IQueryService _queryService;

        public QueryController(IQueryService queryService)
        {
            _queryService = queryService;
        }

        // Get all Queries with optional search query (Admin, NGO roles)
        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetQueries([FromQuery] string searchQuery = "")
        {
            var queries = await _queryService.GetQueriesAsync(searchQuery);
            return Ok(queries);
        }

        // Get Query by Id (Admin, NGO, and Customer roles can access their own queries)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO,User")]
        public async Task<IActionResult> GetQueryById(int id)
        {
            var query = await _queryService.GetQueryByIdAsync(id);
            if (query == null)
                return NotFound("Query not found.");

            // Allow users to see only their own queries
            if (User.IsInRole("User") && query.CustomerId != int.Parse(User.Identity.Name))
                return Forbid();

            return Ok(query);
        }

        // Get Queries for a specific Customer (Admin, NGO, and Customer roles)
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,NGO,User")]
        public async Task<IActionResult> GetQueriesByCustomerId(int customerId)
        {
            if (User.IsInRole("User") && customerId != int.Parse(User.Identity.Name))
                return Forbid();

            var queries = await _queryService.GetQueriesByCustomerIdAsync(customerId);
            return Ok(queries);
        }

        // Add a Query (Admin and NGO roles)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddQuery([FromBody] Query query)
        {
            if (query == null)
                return BadRequest("Query data is required.");

            var addedQuery = await _queryService.AddQueryAsync(query);
            return CreatedAtAction(nameof(GetQueryById), new { id = addedQuery.QueryId }, addedQuery);
        }

        // Update a Query (Admin and NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateQuery(int id, [FromBody] Query updatedQuery)
        {
            if (updatedQuery == null)
                return BadRequest("Updated Query data is required.");

            var query = await _queryService.UpdateQueryAsync(id, updatedQuery);
            if (query == null)
                return NotFound("Query not found.");

            return Ok(query);
        }

        // Delete a Query (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuery(int id)
        {
            var result = await _queryService.DeleteQueryAsync(id);
            if (!result)
                return NotFound("Query not found.");

            return NoContent();
        }
    }
}
