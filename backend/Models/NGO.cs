using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class NGO
    {
        [Key]
        public int NGOId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [MaxLength(50)]
        public string Code { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string? Mission { get; set; }

        public string? Team { get; set; }

        public string? Careers { get; set; }

        public string? Achievements { get; set; }

        public bool IsApproved { get; set; } = false;

        [MaxLength(100)]
        public string Email { get; set; } = null!; // New email field for NGO creation

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Program1> Program1s { get; set; } = new List<Program1>();

        // Link to Account
        public int AccountId { get; set; }
        [ForeignKey("AccountId")]
        public Account? Account { get; set; }
    }
}
