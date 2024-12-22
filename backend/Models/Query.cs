using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class Query
    {
        [Key]
        public int QueryId { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; } = null!;

        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public Program1 Program1 { get; set; } = null!;

        [MaxLength(150)]
        public string Subject { get; set; } = null!;

        public string QueryText { get; set; } = null!;

        public string? ReplyText { get; set; }

        public string Status { get; set; } = "Open";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
