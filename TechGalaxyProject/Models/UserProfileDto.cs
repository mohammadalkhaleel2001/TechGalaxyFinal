namespace TechGalaxyProject.Models
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public List<FollowedRoadmapDto> FollowedRoadmaps { get; set; }

        // هذه الخاصيات للخبراء فقط
        public string? Specialty { get; set; }
        public string? CertificatePath { get; set; }
        public List<RoadmapDto> CreatedRoadmaps { get; set; }
    }
}
