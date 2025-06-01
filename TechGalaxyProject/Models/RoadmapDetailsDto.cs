namespace TechGalaxyProject.Models
{
    public class RoadmapDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<FieldDto> Fields { get; set; }
    }
}
