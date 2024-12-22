using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Program1
    {
        [Key]
        public int ProgramId { get; set; }

        public int NGOId { get; set; }
        [ForeignKey("NGOId")]
        public NGO NGO { get; set; } = null!;

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsUpcoming { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProgramDonation> Donations { get; set; } = new List<ProgramDonation>();

        public ICollection<GalleryImage> GalleryImages { get; set; } = new List<GalleryImage>();

        public ICollection<Query> Queries { get; set; } = new List<Query>();
    }

}
