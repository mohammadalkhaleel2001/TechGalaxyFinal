using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechGalaxyProject.Data.Models
{
    public class FollowedRoadmap
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof (Learner))]
        public string LearnerId { get; set; }
        [ForeignKey(nameof (Roadmap))]
        public int RoadmapId { get; set; }

        public virtual AppUser Learner { get; set; } 
        public virtual Roadmap Roadmap { get; set; } 
    }
}
