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
        public Program1 Program1 { get; set; } = null!;

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        public decimal Amount { get; set; }  // Số tiền đã đóng góp

        public decimal TargetAmount { get; set; }  // Số tiền mục tiêu của chương trình

        public string PaymentStatus { get; set; } = "Pending";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DonationDate { get; set; } = DateTime.UtcNow;

        public ICollection<TransactionHistory> Transactions { get; set; } = new List<TransactionHistory>();

        // Tính số tiền còn lại để đạt mục tiêu (Remaining Amount)
        [NotMapped]  // Không lưu vào cơ sở dữ liệu
        public decimal RemainingAmount => TargetAmount - Amount > 0 ? TargetAmount - Amount : 0;

        // Tính số tiền thừa nếu đóng góp vượt quá mục tiêu (Excess Amount)
        [NotMapped]  // Không lưu vào cơ sở dữ liệu
        public decimal ExcessAmount => Amount > TargetAmount ? Amount - TargetAmount : 0;

        // Tính phần trăm đã đạt được của mục tiêu (Percentage Achieved)
        [NotMapped]  // Không lưu vào cơ sở dữ liệu
        public decimal PercentageAchieved => TargetAmount > 0 ? (Amount / TargetAmount) * 100 : 0;
    }
}
