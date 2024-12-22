using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models{
    public class TransactionHistory
    {
        [Key]
        public int TransactionId { get; set; }

        public int DonationId { get; set; }
        [ForeignKey("DonationId")]
        public ProgramDonation Donation { get; set; } = null!;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string? TransactionDetails { get; set; }
    }
}