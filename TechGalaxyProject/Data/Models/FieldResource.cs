using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechGalaxyProject.Data.Models
{
    public class FieldResource
    {
        [Key]
        public int Id { get; set; }
        public int FieldId { get; set; }
        public virtual Field Field { get; set; }
        public string Link { get; set; }
        public int Order { get; set; }
    }
}
