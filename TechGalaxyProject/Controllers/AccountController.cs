using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using TechGalaxyProject.Data;
using TechGalaxyProject.Data.Models;
using TechGalaxyProject.Models;
using TechGalaxyProject.Services;

namespace TechGalaxyProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;
       



        public AccountController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            AppDbContext db,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IAccountService accountService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            //_configuration = configuration;
            _db = db;
            //_emailSender = emailSender;
            _logger = logger;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var followedRoadmaps = await _db.FollowedRoadmaps
                .Include(f => f.Roadmap)
                .Where(f => f.LearnerId == userId)
                .Select(f => new FollowedRoadmapDto
                {
                    Id = f.RoadmapId,
                    Title = f.Roadmap.Title,
                    Category = f.Roadmap.Category
                })
                .ToListAsync();

            // إذا كان المستخدم خبيراً
            if (user.Role == "Expert")
            {
                var createdRoadmaps = await _db.roadmaps
                    .Where(r => r.CreatedBy == userId)
                    .Select(r => new RoadmapDto
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Category = r.Category,
                        DifficultyLevel = r.DifficultyLevel,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                var profile = new UserProfileDto
                {
                    Name = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    FollowedRoadmaps = followedRoadmaps,
                    Specialty = user.Specialty,
                    CertificatePath = user.CertificatePath,
                    CreatedRoadmaps = createdRoadmaps
                };
                return Ok(profile);
            }
            else
            {
                var profile = new UserProfileDto
                {
                    Name = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    FollowedRoadmaps = followedRoadmaps
                };
                return Ok(profile);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            if (model.Name != null)
                user.UserName = model.Name;

            if (model.Email != null)
            {
                user.Email = model.Email;
                await _userManager.UpdateAsync(user);
            }

            // التحديث فقط للخبراء
            if (user.Role == "Expert")
            {
                if (model.Specialty != null)
                    user.Specialty = model.Specialty;

                if (model.CertificateFile != null)
                {
                    // معالجة ملف الشهادة

                   // حذف الملف القديم إذا كان موجوداً
                  if (!string.IsNullOrEmpty(user.CertificatePath))
                    {
                        await _accountService.DeleteFileAsync(user.CertificatePath);
                    }

                    // حفظ الملف الجديد
                    // var newFilePath = await _accountService.SaveFileAsync(model.CertificateFile, "certificates");
                    var newFilePath = await _accountService.SaveFileAsync(model.CertificateFile, "uploads/certificates");

                    user.CertificatePath = newFilePath;
                }
            }
            else if (model.Specialty != null || model.CertificateFile != null)
            {
                return BadRequest("Only experts can update specialty and certificate");
            }

            await _userManager.UpdateAsync(user);
            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser([FromForm] dtoNewUser user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.RegisterUserAsync(user, Request);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                message = result.Message,
                requiresApproval = result.RequiresApproval
            });
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LogIn([FromBody] dtoLogin login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _accountService.LoginAsync(login);

            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result.Data);
        }





        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            _logger.LogInformation(" ForgotPassword called for: {Email}", model.Email);

            var result = await _accountService.ForgotPasswordAsync(model);

            if (!result.Success)
            {
                _logger.LogWarning(" ForgotPassword failed: {Message}", result.Message);
                return BadRequest(result.Message);
            }

            _logger.LogInformation(" ForgotPassword successful: {Email}", model.Email);
            return Ok(new { message = result.Message });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var result = await _accountService.ResetPasswordAsync(model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new { message = result.Message });
        }

        [HttpGet("PendingExpertVerifications")]
        public async Task<IActionResult> GetPendingExpertVerifications()
        {
            var pendingRequests = await _accountService.GetPendingExpertVerificationsAsync();
            return Ok(pendingRequests);
        }


        [HttpPost("ReviewExpert")]
        public async Task<IActionResult> ReviewExpert([FromBody] ExpertReviewDto model)
        {
            var result = await _accountService.ReviewExpertAsync(model);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(new { message = result.Message });
        }




        [HttpGet("GetCurrentUser")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.UserName,
                user.Email,
                user.Role
            });
        }

        [HttpPost("CheckUserExists")]
        public async Task<IActionResult> CheckUserExists(dtoCheckUser user)
        {
            var existingUser = await _userManager.FindByNameAsync(user.userName);
            return Ok(new { exists = existingUser != null });
        }
    }
}