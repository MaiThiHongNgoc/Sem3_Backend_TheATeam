using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Partner {
        [Key]
        public int PartnerId { get; set; }

        [Required, MaxLength(100)]
        public string CompanyName { get; set; }

        [Required]
        public string BankAccount { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
