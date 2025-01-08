using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class GalleryImage
    {
        [Key]
        public int ImageId { get; set; }

        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public Program1? Program1 { get; set; } 

        [MaxLength(255)]
        public string? ImageUrl { get; set; }
        public string FileName { get; set; } = null!;

        public string? Caption { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
