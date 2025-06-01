namespace TechGalaxyProject.Models
{
    public class FieldDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public List<string> Resources { get; set; }  // قائمة من روابط
    }
}
