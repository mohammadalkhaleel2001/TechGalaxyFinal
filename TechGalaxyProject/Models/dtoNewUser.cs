using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TechGalaxyProject.Models
{
    public class dtoNewUser
    {
        [Required]
        public string userName { get; set; } = default!;

        [Required]
        public string password { get; set; } = default!;

        [Required]
        public string email { get; set; } = default!;

      //  public string phonNember { get; set; } = default!;

        [Required]
        public string Role { get; set; } = default!;

        public string? Specialty { get; set; }

        public IFormFile? CertificateFile { get; set; }
    }
}
