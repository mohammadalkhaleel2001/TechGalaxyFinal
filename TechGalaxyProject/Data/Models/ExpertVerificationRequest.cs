using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechGalaxyProject.Data.Models
{
    public class ExpertVerificationRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Expert))]
        public string UserId { get; set; } = default!;

        [ForeignKey(nameof(Admin))]
        public string? ReviewedBy { get; set; }

        public virtual AppUser Expert { get; set; } = default!;

        public virtual AppUser? Admin { get; set; }

        public string Specialty { get; set; } = default!;

        public string CertificatePath { get; set; } = default!;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public DateTime? ReviewedAt { get; set; }

        public string? AdminNote { get; set; }
    }
}
