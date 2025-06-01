using System.ComponentModel.DataAnnotations;

namespace TechGalaxyProject.Data.Models
{
    public class RoadmapLike
    {
        [Key] 
        public int Id { get; set; }

        public int RoadmapId { get; set; }
        public virtual Roadmap Roadmap { get; set; }

        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
    }
}
