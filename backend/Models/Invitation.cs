using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models{
    public class Invitation
    {
        [Key]
        public int InvitationId { get; set; }

        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public Customer Sender { get; set; } = null!;

        [MaxLength(100)]
        public string RecipientEmail { get; set; } = null!;

        public string? Message { get; set; }

        public string Status { get; set; } = "Pending";

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}