using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Donation {
        [Key]
        public int DonationId { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int CauseId { get; set; }
        public Cause Cause { get; set; }

        public int NGOId { get; set; }
        public NGO NGO { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        public string PaymentStatus { get; set; } = "Pending"; // 'Pending', 'Completed', 'Failed'
        public string DistributionStatus { get; set; } = "Pending"; // 'Pending', 'Distributed'

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
