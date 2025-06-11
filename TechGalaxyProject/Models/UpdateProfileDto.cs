namespace TechGalaxyProject.Models
{
    public class UpdateProfileDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }

        // هذه الخاصيات للخبراء فقط
        public string? Specialty { get; set; }
        public IFormFile? CertificateFile { get; set; }
    }
}
