using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Payment {
        [Key]
        public int PaymentId { get; set; }

        public int DonationId { get; set; }
        public Donation Donation { get; set; }

        public string PaymentMethod { get; set; } = "CreditCard"; // 'CreditCard', 'BankTransfer'

        [Required]
        public string PaymentReference { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    }
}
