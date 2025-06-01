using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechGalaxyProject.Data.Models
{
    public class CompletedFields
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        public string LearnerId { get; set; }
        public virtual AppUser User { get; set; }
        [ForeignKey(nameof (field))]
        public int FieldId { get; set; }
        public virtual Field field { get; set; }
        public DateTime CompletedAt { get; set; }

    }
}
