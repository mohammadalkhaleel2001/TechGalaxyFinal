using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechGalaxyProject.Data;
using TechGalaxyProject.Data.Models;
using TechGalaxyProject.Models;

namespace TechGalaxyProject.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender;


        public AccountService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db,
            IWebHostEnvironment env,
             IConfiguration configuration,
         IEmailSender emailSender
)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _env = env;
            _configuration = configuration;
            _emailSender = emailSender;
        }

        public async Task<(bool Success, string Message, bool RequiresApproval)> RegisterUserAsync(dtoNewUser user, HttpRequest request)
        {
            if (user.Role != "Expert" && user.Role != "Learner")
                return (false, "Role must be either 'Expert' or 'Learner'", false);

            if (user.Role == "Expert")
            {
                if (string.IsNullOrWhiteSpace(user.Specialty))
                    return (false, "Specialty is required for Experts.", false);

                if (user.CertificateFile == null || user.CertificateFile.Length == 0)
                    return (false, "Certificate file is required for Experts.", false);
            }

            var existingUser = await _userManager.FindByEmailAsync(user.email);
            if (existingUser != null)
                return (false, "Email is already registered.", false);

            var appUser = new AppUser
            {
                UserName = user.userName,
                Email = user.email,
                Role = user.Role,
                IsVerified = user.Role == "Expert" ? false : true
            };

            var result = await _userManager.CreateAsync(appUser, user.password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors, false);
            }

            if (!await _roleManager.RoleExistsAsync(user.Role))
                await _roleManager.CreateAsync(new IdentityRole(user.Role));

            await _userManager.AddToRoleAsync(appUser, user.Role);

            if (user.Role == "Expert")
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "certificates");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(user.CertificateFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await user.CertificateFile.CopyToAsync(stream);

                var baseUrl = $"{request.Scheme}://{request.Host}";
                var certificateUrl = $"{baseUrl}/uploads/certificates/{fileName}";

                var verificationRequest = new ExpertVerificationRequest
                {
                    UserId = appUser.Id,
                    Specialty = user.Specialty!,
                    CertificatePath = certificateUrl,
                    SubmittedAt = DateTime.UtcNow,
                    Status = "Pending"
                };

                _db.ExpertVerificationRequests.Add(verificationRequest);
                await _db.SaveChangesAsync();

                return (true, "Registration successful. Certificate submitted for approval.", true);
            }

            return (true, "User registered successfully", false);
        }
        public async Task<(bool Success, string Message, object? Data)> LoginAsync(dtoLogin login)
        {
            var user = await _userManager.FindByEmailAsync(login.email);
            if (user == null)
                return (false, "User not found", null);

            if (!await _userManager.CheckPasswordAsync(user, login.password))
                return (false, "Invalid password", null);

            if (user.Role == "Expert" && !user.IsVerified)
                return (false, "Your account is pending verification. Please wait for admin approval.", null);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["JWT:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
                return (false, "JWT Secret Key not configured", null);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var resultData = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo,
                userName = user.UserName,
                email = user.Email,
                role = roles.FirstOrDefault() ?? "Unknown"
            };

            return (true, "Login successful", resultData);
        }
        public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto model)
        {
            var users = await _userManager.Users
                .Where(u => u.Email == model.Email)
                .ToListAsync();

            if (users.Count == 0)
            {
                return (false, "Email not found.");
            }

            if (users.Count > 1)
            {
                return (false, "Multiple users found with this email. Please contact support.");
            }

            var user = users.First();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var resetUrl = $"https://68311f7890065681810ad943--steady-rugelach-10f3fa.netlify.app/resetPassword.html?email={model.Email}&token={encodedToken}";

            var htmlMessage = _emailSender is SmtpEmailSender smtp
                ? smtp.GenerateResetPasswordEmailBody(user.UserName ?? user.Email, resetUrl)
                : $"Click the link to reset your password: <a href='{resetUrl}'>Reset</a>";

            try
            {
                await _emailSender.SendEmailAsync(model.Email, "Reset Your Password", htmlMessage);
            }
            catch
            {
                return (false, "Failed to send email. Please try again.");
            }

            return (true, "Password reset link has been sent to your email.");
        }
        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (false, "User not found");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            return (true, "Password has been reset successfully.");
        }
        public async Task<List<PendingExpertVerificationDto>> GetPendingExpertVerificationsAsync()
        {
            return await _db.ExpertVerificationRequests
                .Include(r => r.Expert)
                .Where(r => r.Status == "Pending")
                .Select(r => new PendingExpertVerificationDto
                {
                    RequestId = r.Id,
                    UserId = r.Expert.Id,
                    UserName = r.Expert.UserName ?? string.Empty,
                    Email = r.Expert.Email ?? string.Empty,
                    Specialty = r.Specialty,
                    CertificateUrl = r.CertificatePath,
                    SubmittedAt = r.SubmittedAt
                })
                .ToListAsync();
        }
        public async Task<(bool Success, string Message)> ReviewExpertAsync(ExpertReviewDto model)
        {
            var request = await _db.ExpertVerificationRequests
                .Include(r => r.Expert)
                .FirstOrDefaultAsync(r => r.Id == model.Id && r.Status == "Pending");

            if (request == null)
                return (false, "Request not found or already processed.");

            request.Status = model.Approve ? "Approved" : "Rejected";
            request.ReviewedAt = DateTime.UtcNow;

            var expert = request.Expert;
            string subject, body;

            if (model.Approve)
            {
                expert.IsVerified = true;
                subject = "✅ Expert Verification Approved";
                body = $"Hello {expert.UserName},<br><br>" +
                       $"Your expert verification request has been <b>approved</b>. You now have full access to the platform as an expert.<br><br>" +
                       $"Best regards,<br>TechGalaxy Team";
            }
            else
            {
                expert.IsVerified = false;
                subject = "❌ Expert Verification Rejected";
                body = $"Hello {expert.UserName},<br><br>" +
                       $"Unfortunately, your expert verification request has been <b>rejected</b>. Please ensure your certificate is valid and try again later.<br><br>" +
                       $"Best regards,<br>TechGalaxy Team";

                _db.ExpertVerificationRequests.Remove(request);
                _db.Users.Remove(expert);
            }

            try
            {
                await _emailSender.SendEmailAsync(expert.Email, subject, body);
            }
            catch
            {
                // تجاهل الخطأ بدون إخراج استثناء
            }

            await _db.SaveChangesAsync();
            return (true, model.Approve ? "Expert approved." : "Expert rejected.");
        }


    }
}
