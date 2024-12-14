using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? Username { get; set; }
        public string? Password { get; set; } 

        public bool IsActive { get; set; } = true;

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public Customer Customer { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}