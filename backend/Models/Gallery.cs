using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Gallery {
        [Key]
        public int ImageId { get; set; }

        public int NGOId { get; set; }
        public NGO NGO { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string Caption { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
