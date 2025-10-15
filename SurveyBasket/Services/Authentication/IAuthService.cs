namespace SurveyBasket.Services.Authentication;

public interface IAuthService : IScopedService
{
    public Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    public Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken);
    public Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    public Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken);
    public Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, CancellationToken cancellationToken);

}
