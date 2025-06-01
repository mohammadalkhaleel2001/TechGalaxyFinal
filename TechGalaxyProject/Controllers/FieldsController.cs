using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechGalaxyProject.Data;
using TechGalaxyProject.Data.Models;
using TechGalaxyProject.Models;

namespace TechGalaxyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Expert")]
    public class FieldsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FieldsController(AppDbContext db)
        {
            _db = db;
        }

       

        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateField(int id, [FromBody] UpdateFieldDto dto)
        {
            var field = await _db.fields
                .Include(f => f.roadmap)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (field == null)
                return NotFound("Field not found");

            if (field.roadmap.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid("You can only update your own fields");

            field.Title = dto.Title;
            field.Description = dto.Description;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                field.Id,
                field.Title,
                field.Description,
                field.Order
            });
        }

        [HttpPost("add-resource")]
        public async Task<IActionResult> AddResource([FromBody] AddResourceDto dto)
        {
            var field = await _db.fields
                .Include(f => f.roadmap)
                .Include(f => f.Resources)
                .FirstOrDefaultAsync(f => f.Id == dto.FieldId);

            if (field == null)
                return NotFound("Field not found");

            if (field.roadmap.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid("You can only add resources to your own fields");

            var fieldResource = new FieldResource
            {
                FieldId = dto.FieldId,
                Link = dto.Link,
                Order = field.Resources?.Count ?? 0 + 1
            };

            _db.fieldResources.Add(fieldResource);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                field.Id,
                field.Title,
                field.Description,
                field.Order,
                Resources = field.Resources?.Select(r => r.Link) ?? new List<string>()
            });
        }

        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var field = await _db.fields
                .Include(f => f.roadmap)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (field == null)
                return NotFound("Field not found");

            if (field.roadmap.CreatedBy != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid("You can only delete your own fields");

            _db.fields.Remove(field);
            await _db.SaveChangesAsync();

            return Ok("Field deleted successfully");
        }
    }
}
