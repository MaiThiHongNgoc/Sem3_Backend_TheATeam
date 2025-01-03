using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class ProgramDonation
    {
        [Key]
        public int DonationId { get; set; }

        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public Program1? Program1 { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public decimal Amount { get; set; }  // Số tiền đã đóng góp

        public string PaymentStatus { get; set; } = "Pending";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        public ICollection<TransactionHistory> Transactions { get; set; } = new List<TransactionHistory>();
    }
}
