namespace TechGalaxyProject.Models
{
    public class PendingExpertVerificationDto
    {
        public int RequestId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public string CertificateUrl { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
    }

}
