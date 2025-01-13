using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Program1
    {
        [Key]
        public int ProgramId { get; set; }

        [Required]  // Ensure NGOId is required
        public int NGOId { get; set; }
        [ForeignKey("NGOId")]
        public NGO? NGO { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsUpcoming { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // TargetAmount: Số tiền mục tiêu của chương trình
        public decimal TargetAmount { get; set; }

        public ICollection<ProgramDonation> Donations { get; set; } = new List<ProgramDonation>();

        public ICollection<GalleryImage> GalleryImages { get; set; } = new List<GalleryImage>();

        public ICollection<Query> Queries { get; set; } = new List<Query>();

        // Tính tổng số tiền đã được đóng góp
        [NotMapped]
        public decimal TotalDonatedAmount => Donations.Sum(d => d.Amount);

        // Tính số tiền còn lại để đạt mục tiêu (Remaining Amount)
        [NotMapped]
        public decimal RemainingAmount => TargetAmount - TotalDonatedAmount > 0 ? TargetAmount - TotalDonatedAmount : 0;

        // Tính phần trăm đã đạt được của mục tiêu (Percentage Achieved)
        [NotMapped]
        public decimal PercentageAchieved => TargetAmount > 0 ? (TotalDonatedAmount / TargetAmount) * 100 : 0;

        // Tính số tiền thừa nếu đóng góp vượt quá mục tiêu (Excess Amount)
        [NotMapped]
        public decimal ExcessAmount => TotalDonatedAmount > TargetAmount ? TotalDonatedAmount - TargetAmount : 0;

         [NotMapped]
        public object ProgramDonations { get; internal set; }
         [NotMapped]
        public object ProgramDonation { get; internal set; }
    }
}
