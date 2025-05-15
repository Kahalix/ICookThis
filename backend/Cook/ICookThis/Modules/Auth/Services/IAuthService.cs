using ICookThis.Modules.Auth.Dtos;

namespace ICookThis.Modules.Auth.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest dto);
        Task<bool> ConfirmEmailAsync(Guid token);
        Task<AuthResponse> LoginAsync(LoginRequest dto);
        Task ForgotPasswordAsync(ForgotPasswordRequest dto);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest dto);
        Task CleanupPendingAsync();
    }
}