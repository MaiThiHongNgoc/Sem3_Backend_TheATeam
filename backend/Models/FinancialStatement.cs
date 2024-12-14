using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class FinancialStatement {
        [Key]
        public int StatementId { get; set; }

        public int NGOId { get; set; }
        public NGO NGO { get; set; }

        public int DonationId { get; set; }
        public Donation Donation { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime StatementDate { get; set; } = DateTime.UtcNow;

        public string Note { get; set; }
    }
}
