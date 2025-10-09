using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Services.Authentication
{
    public interface IAuthService : IScopedService
    {
        public Task<AuthResponse> Login(LoginRequest request);
        public Task<(bool succeeded, IEnumerable<IdentityError>? errors)> Register(RegisterRequest request);
    }
}
