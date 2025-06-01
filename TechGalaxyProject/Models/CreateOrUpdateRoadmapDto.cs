using Microsoft.AspNetCore.Http;

namespace TechGalaxyProject.Models
{
    public class CreateOrUpdateRoadmapDto
    {
        public string Tag { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile CoverImage { get; set; }  // إضافة ملف الصورة
        public string Category { get; set; }
        public string StepTitle { get; set; }
        public string StepDescription { get; set; }
        public string DifficultyLevel { get; set; }  // إضافة حقل الصعوبة
    }
}
