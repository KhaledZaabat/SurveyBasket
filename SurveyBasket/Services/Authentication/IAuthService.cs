using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Services.Authentication
{
    public interface IAuthService : IScopedService
    {
        public Task<AuthResponse> LoginAsync(LoginRequest request);
        public Task<AuthResponse> RefreshAsync(RefreshRequest request);
        public Task<(bool Succeeded, IEnumerable<IdentityError>? Errors)> RegisterAsync(RegisterRequest request);
        // public Task<TokenResponse> Refresh(RefreshRequest request);
    }
}
