using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionHistoryController : ControllerBase
    {
        private readonly ITransactionHistoryService _transactionHistoryService;

        public TransactionHistoryController(ITransactionHistoryService transactionHistoryService)
        {
            _transactionHistoryService = transactionHistoryService;
        }

        // Get all TransactionHistories with optional search query (Admin, User, and NGO roles)
        [HttpGet]
        public async Task<IActionResult> GetTransactionHistories([FromQuery] string searchQuery = "")
        {
            var transactionHistories = await _transactionHistoryService.GetTransactionHistoriesAsync(searchQuery);
            return Ok(transactionHistories);
        }

        // Get TransactionHistory by Id (Admin, User, and NGO roles can access)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionHistoryById(int id)
        {
            var transactionHistory = await _transactionHistoryService.GetTransactionHistoryByIdAsync(id);
            if (transactionHistory == null)
                return NotFound("Transaction History not found.");

            return Ok(transactionHistory);
        }

        // Add a TransactionHistory (Admin and NGO roles)
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> AddTransactionHistory([FromBody] TransactionHistory transactionHistory)
        {
            if (transactionHistory == null)
                return BadRequest("Transaction History data is required.");

            var addedTransactionHistory = await _transactionHistoryService.AddTransactionHistoryAsync(transactionHistory);
            return CreatedAtAction(nameof(GetTransactionHistoryById), new { id = addedTransactionHistory.TransactionId }, addedTransactionHistory);
        }

        // Update a TransactionHistory (Admin and NGO roles)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateTransactionHistory(int id, [FromBody] TransactionHistory updatedTransactionHistory)
        {
            if (updatedTransactionHistory == null)
                return BadRequest("Updated Transaction History data is required.");

            var transactionHistory = await _transactionHistoryService.UpdateTransactionHistoryAsync(id, updatedTransactionHistory);
            if (transactionHistory == null)
                return NotFound("Transaction History not found.");

            return Ok(transactionHistory);
        }

        // Delete a TransactionHistory (Admin role only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTransactionHistory(int id)
        {
            var result = await _transactionHistoryService.DeleteTransactionHistoryAsync(id);
            if (!result)
                return NotFound("Transaction History not found.");

            return NoContent();
        }
    }
}
