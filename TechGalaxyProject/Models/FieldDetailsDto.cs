namespace TechGalaxyProject.Models
{
    public class FieldDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public List<string> Resources { get; set; }
        public bool IsCompleted { get; set; }
       // public bool CanMark { get; set; }  // هل يمكن للمستخدم وضع علامة على هذا الحقل
    }
}
