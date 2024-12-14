using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data {
    public class MyAppContext : DbContext {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens {get; set;}
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Program1> Program1s { get; set; }
        public DbSet<ProgramRegistration> ProgramRegistrations { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<FinancialStatement> FinancialStatements { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<NGO> NGOs { get; set; }
        public DbSet<Query> Queries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "User" },
                new Role { RoleId = 3, RoleName = "NGO" }
            );
        }
    }
}
