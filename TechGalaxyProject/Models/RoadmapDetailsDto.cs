namespace TechGalaxyProject.Models
{
    public class RoadmapDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Tag { get; set; }
        public string DifficultyLevel { get; set; }
        public string CoverImageUrl { get; set; }
        public int LikesCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public bool IsFollowed { get; set; }
        public List<FieldDetailsDto> Fields { get; set; }
    }
}
