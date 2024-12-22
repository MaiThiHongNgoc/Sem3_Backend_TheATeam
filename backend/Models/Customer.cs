using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public Account Account { get; set; } = null!;

        [MaxLength(50)]
        public string? FirstName { get; set; }  // Cho phép null nếu Facebook không cung cấp

        [MaxLength(50)]
        public string? LastName { get; set; }  // Cho phép null nếu Facebook không cung cấp
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string Gender { get; set; } = "Other";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProgramDonation> ProgramDonations { get; set; } = new List<ProgramDonation>();

        public ICollection<Query> Queries { get; set; } = new List<Query>();
    }
}