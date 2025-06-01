using Microsoft.AspNetCore.Http;
using TechGalaxyProject.Models;

namespace TechGalaxyProject.Services
{
    public interface IAccountService
    {
        Task<(bool Success, string Message, bool RequiresApproval)> RegisterUserAsync(dtoNewUser user, HttpRequest request);
        Task<(bool Success, string Message, object? Data)> LoginAsync(dtoLogin login);
        Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto model);
        Task<List<PendingExpertVerificationDto>> GetPendingExpertVerificationsAsync();
        Task<(bool Success, string Message)> ReviewExpertAsync(ExpertReviewDto model);



    }
}
