using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class ProgramRegistration {
        [Key]
        public int RegistrationId { get; set; }

        public int ProgramId { get; set; }
        public Program1 Program1 { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
