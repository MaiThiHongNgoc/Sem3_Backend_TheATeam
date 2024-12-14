using System.ComponentModel.DataAnnotations;

namespace backend.Models {
    public class Role {
        [Key]
        public int RoleId { get; set; }

        [Required, MaxLength(50)]
        public string RoleName { get; set; }

        // Relationships
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
