using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechGalaxyProject.Data.Models
{
    public class Roadmap
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public string Category { get; set; }
        public string Tag { get; set; }
        public string DifficultyLevel { get; set; }
        public int LikesCount { get; set; }

        [ForeignKey(nameof(User))]
        public string CreatedBy { get; set; }
        public virtual AppUser User { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Field> fields { get; set; }
        public virtual ICollection<FollowedRoadmap> followedBy { get; set; }
        public virtual ICollection<RoadmapLike> Likes { get; set; }

    }
}
