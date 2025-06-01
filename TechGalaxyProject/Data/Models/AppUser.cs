using Microsoft.AspNetCore.Identity;

namespace TechGalaxyProject.Data.Models
{
    public class AppUser : IdentityUser
    {
        public string Role { get; set; } = default!;
        public bool IsVerified { get; set; }

       
        public string? Specialty { get; set; }

        public string? CertificatePath { get; set; }

      
        public virtual ICollection<CompletedFields> completedFields { get; set; } = new List<CompletedFields>();
        public virtual ExpertVerificationRequest? request { get; set; }
        public virtual ICollection<ExpertVerificationRequest> requests { get; set; } = new List<ExpertVerificationRequest>();
        public virtual ICollection<Roadmap> createdRoadmaps { get; set; } = new List<Roadmap>();
        public virtual ICollection<FollowedRoadmap> followedRoadmaps { get; set; } = new List<FollowedRoadmap>();
        public virtual ICollection<RoadmapLike> likedRoadmaps { get; set; } = new List<RoadmapLike>();

    }
}
