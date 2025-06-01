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
    public class FollowedRoadmapsController : ControllerBase
    {
        public FollowedRoadmapsController (AppDbContext db)
        {
            _db = db;
        }
        private readonly AppDbContext _db;
        [HttpPost("{roadmapId}")]
        public async Task<IActionResult> FollowRoadmap(int roadmapId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool alreadyFollowing = await _db.FollowedRoadmaps.AnyAsync(f => f.RoadmapId == roadmapId && f.LearnerId == userId);
            if (alreadyFollowing)
                return BadRequest("You are already following this roadmap.");

            var follow = new FollowedRoadmap
            {
                RoadmapId = roadmapId,
                LearnerId = userId
            };
            _db.FollowedRoadmaps.Add(follow);
            await _db.SaveChangesAsync();
            return Ok("Roadmap followed.");
        }
        [HttpDelete("{roadmapId}")]
        public async Task<IActionResult> UnfollowRoadmap(int roadmapId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followed = await _db.FollowedRoadmaps.FirstOrDefaultAsync(f => f.RoadmapId == roadmapId && f.LearnerId == userId);
            if (followed == null)
                return NotFound("You are not following this roadmap.");

            _db.FollowedRoadmaps.Remove(followed);
            await _db.SaveChangesAsync();
            return Ok("Unfollowed.");
        }
        [HttpGet]
        public async Task<IActionResult> GetFollowedRoadmaps()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followed = await _db.FollowedRoadmaps
                .Include(f => f.Roadmap)
                .Where(f => f.LearnerId == userId)
                .Select(f => new
                {
                    f.Roadmap.Id,
                    f.Roadmap.Title,
                    f.Roadmap.Description
                })
                .ToListAsync();

            return Ok(followed);
        }

    }
}
