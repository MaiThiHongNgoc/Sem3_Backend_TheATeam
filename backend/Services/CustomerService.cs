using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly MyAppContext _context;

        public CustomerService(MyAppContext context)
        {
            _context = context;
        }
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.Account) // Bao gồm dữ liệu của Account
                .ToListAsync();
        }


        // Add Customer
        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        // Update Customer
        public async Task<Customer> UpdateCustomerAsync(int id, Customer updatedCustomer)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return null;

            customer.FirstName = updatedCustomer.FirstName;
            customer.LastName = updatedCustomer.LastName;
            customer.DateOfBirth = updatedCustomer.DateOfBirth;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;
            customer.Address = updatedCustomer.Address;
            customer.Gender = updatedCustomer.Gender;
            customer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return customer;
        }

        // Delete Customer
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return false;

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get Customer by Id
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Account)
                .Include(c => c.ProgramDonations)
                .Include(c => c.Queries)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        // Get all Customers with search functionality
        public async Task<List<Customer>> GetCustomersAsync(string searchQuery = "")
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => c.FirstName.Contains(searchQuery) || c.LastName.Contains(searchQuery));
            }

            return await query.ToListAsync();
        }
        public Customer GetCustomerByAccountId(int accountId)
        {
            return _context.Customers.FirstOrDefault(c => c.AccountId == accountId);
        }
    }

    public interface ICustomerService
    {
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(int id, Customer updatedCustomer);
        Task<bool> DeleteCustomerAsync(int id);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<List<Customer>> GetCustomersAsync(string searchQuery = "");
        Task<List<Customer>> GetAllCustomersAsync();
        Customer GetCustomerByAccountId(int accountId);
    }
}
