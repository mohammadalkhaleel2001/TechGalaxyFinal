using System.ComponentModel.DataAnnotations;

namespace TechGalaxyProject.Models
{
    public class dtoLogin
    {
        [Required]
        public string email { get; set; } = default!;

        [Required]
        public string password { get; set; } = default!;
    }

}
