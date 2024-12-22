using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Partner
    {
        [Key]
        public int PartnerId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? LogoUrl { get; set; }

        public string? Description { get; set; }

        public bool IsApproved { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
