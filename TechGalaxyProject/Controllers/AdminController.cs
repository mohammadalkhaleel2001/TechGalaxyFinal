using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechGalaxyProject.Data;

namespace TechGalaxyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        
        [HttpGet("verification-requests")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _db.ExpertVerificationRequests
                .Include(r => r.Expert)
                .Where(r => r.Status == "Pending") 
                .Select(r => new
                {
                    r.Id,
                    ExpertId = r.Expert.Id,
                    ExpertUsername = r.Expert.UserName,
                    r.SubmittedAt
                })
                .ToListAsync();

            return Ok(requests);
        }

        
        [HttpPost("approve/{requestId}")]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var request = await _db.ExpertVerificationRequests
                .Include(r => r.Expert)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            request.Status = "Approved"; 
            request.ReviewedAt = DateTime.Now;
            request.ReviewedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.Expert.IsVerified = true;

            await _db.SaveChangesAsync();
            return Ok("Expert approved and verified.");
        }

      
        [HttpPost("reject/{requestId}")]
        public async Task<IActionResult> RejectRequest(int requestId, [FromBody] string adminNote)
        {
            var request = await _db.ExpertVerificationRequests
                .Include(r => r.Expert)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return NotFound("Request not found");

            request.Status = "Rejected"; 
            request.ReviewedAt = DateTime.Now;
            request.ReviewedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.AdminNote = adminNote;

            await _db.SaveChangesAsync();
            return Ok("Verification request rejected.");
        }
    }
}
