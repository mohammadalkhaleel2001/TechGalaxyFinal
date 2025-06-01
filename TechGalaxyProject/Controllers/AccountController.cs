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