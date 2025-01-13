using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class MyAppContext : DbContext
    {
        public MyAppContext(DbContextOptions<MyAppContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Program1> Program1s { get; set; }
        public DbSet<ProgramDonation> ProgramDonations { get; set; }
        public DbSet<GalleryImage> GalleryImages { get; set; }
        public DbSet<NGO> NGOs { get; set; }
        public DbSet<Query> Queries { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "User" },
                new Role { RoleId = 3, RoleName = "NGO" }
            );

            modelBuilder.Entity<ProgramDonation>()
                .HasOne(pd => pd.Program1)
                .WithMany(p => p.Donations)
                .HasForeignKey(pd => pd.ProgramId);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Account)
                .WithOne(a => a.Customer)
                .HasForeignKey<Customer>(c => c.AccountId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
