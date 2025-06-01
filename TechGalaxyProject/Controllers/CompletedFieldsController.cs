using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechGalaxyProject.Data;
using TechGalaxyProject.Data.Models;

namespace TechGalaxyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Learner")]
    public class CompletedFieldsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CompletedFieldsController(AppDbContext db)
        {
            _db = db;
        }
        [HttpPost("{fieldId}")]
        public async Task<IActionResult> MarkAsCompleted(int fieldId)
        {
            int learnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            bool alreadyCompleted = await _db.completedFields
                .AnyAsync(c => c.FieldId == fieldId && c.LearnerId .Equals( learnerId));

            if (alreadyCompleted)
                return BadRequest("Field already marked as completed.");

            var completed = new CompletedFields
            {
                FieldId = fieldId,
                LearnerId = learnerId.ToString()
            };

            _db.completedFields.Add(completed);
            await _db.SaveChangesAsync();
            return Ok("Field marked as completed.");
        }

       
        [HttpDelete("{fieldId}")]
        public async Task<IActionResult> UnmarkAsCompleted(int fieldId)
        {
            int learnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var completed = await _db.completedFields
                .FirstOrDefaultAsync(c => c.FieldId == fieldId && c.LearnerId .Equals( learnerId));

            if (completed == null)
                return NotFound("Field is not marked as completed.");

            _db.completedFields.Remove(completed);
            await _db.SaveChangesAsync();
            return Ok("Field unmarked.");
        }

        
        [HttpGet]
        public async Task<IActionResult> GetCompletedFields()
        {
            int learnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var completedFields = await _db.completedFields
                .Include(c => c.field)
                .Where(c => c.LearnerId .Equals( learnerId))
                .Select(c => new
                {
                    c.field.Id,
                    c.field.Title,
                    c.field.Description
                })
                .ToListAsync();

            return Ok(completedFields);
        }
    }
}
