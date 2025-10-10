namespace SurveyBasket.Services.Authentication
{
    public interface IAuthService : IScopedService
    {
        public Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        public Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request);
        public Task<Result> RegisterAsync(RegisterRequest request);
        // public Task<TokenResponse> Refresh(RefreshRequest request);
    }
}
