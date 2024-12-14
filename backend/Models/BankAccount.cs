using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class BankAccount {
        [Key]
        public int BankAccountId { get; set; }

        public int NGOId { get; set; }
        public NGO NGO { get; set; }

        [Required, MaxLength(100)]
        public string BankName { get; set; }

        [Required, MaxLength(50)]
        public string AccountNumber { get; set; }

        [Required, MaxLength(100)]
        public string AccountHolderName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
