using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/bankaccounts")]
    public class BankAccountController : ControllerBase
    {
        private readonly MyAppContext _context;

        public BankAccountController(MyAppContext context)
        {
            _context = context;
        }

        // Lấy tất cả các tài khoản ngân hàng
        [HttpGet]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetAllBankAccounts()
        {
            var bankAccounts = await _context.BankAccounts.Include(b => b.NGO).ToListAsync();
            return Ok(bankAccounts);
        }

        // Lấy tài khoản ngân hàng theo ID
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> GetBankAccountById(int id)
        {
            var bankAccount = await _context.BankAccounts.Include(b => b.NGO).FirstOrDefaultAsync(b => b.BankAccountId == id);
            if (bankAccount == null)
                return NotFound("Bank account not found.");

            return Ok(bankAccount);
        }

        // Thêm tài khoản ngân hàng mới
        [HttpPost]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> CreateBankAccount([FromBody] BankAccount request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.BankAccounts.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBankAccountById), new { id = request.BankAccountId }, request);
        }

        // Cập nhật tài khoản ngân hàng
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,NGO")]
        public async Task<IActionResult> UpdateBankAccount(int id, [FromBody] BankAccount request)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
                return NotFound("Bank account not found.");

            bankAccount.BankName = request.BankName;
            bankAccount.AccountNumber = request.AccountNumber;
            bankAccount.AccountHolderName = request.AccountHolderName;

            await _context.SaveChangesAsync();
            return Ok("Bank account updated successfully.");
        }

        // Xóa tài khoản ngân hàng
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
                return NotFound("Bank account not found.");

            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
            return Ok("Bank account deleted successfully.");
        }
    }
}
