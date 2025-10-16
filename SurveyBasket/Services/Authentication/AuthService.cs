using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Auhtentication_Providers;
using SurveyBasket.Shared.Errors;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services.Authentication
{
    public class AuthService(
        IJwtProvider jwtProvider,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthService> logger,
        IEmailSender emailService) : IAuthService
    {
        private const int RefreshTokenLifetimeDays = 14;

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                logger.LogWarning("Login failed: user with email {Email} not found", request.Email);
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials());
            }

            if (!user.EmailConfirmed)
            {
                logger.LogWarning("Login failed: email {Email} not confirmed", request.Email);
                return Result.Failure<AuthResponse>(UserError.EmailNotConfirmed());
            }

            var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!result.Succeeded)
            {
                logger.LogWarning("Login failed: invalid credentials for email {Email}", request.Email);
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials());
            }

            var jwtToken = jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken.Token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = refreshToken.ExpiresAt,
                UserId = user.Id
            });

            await userManager.UpdateAsync(user);

            logger.LogInformation("User {Email} logged in successfully", request.Email);

            return Result.Success<AuthResponse>(new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                JwtToken: jwtToken,
                RefreshToken: refreshToken
            ));
        }

        public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Refreshing JWT token for refresh token request");

            string? userId = jwtProvider.ValidateToken(request.JwtToken);
            if (userId is null)
            {
                logger.LogWarning("JWT refresh failed: invalid or expired JWT token");
                return Result.Failure<AuthResponse>(UserError.InvalidToken("Invalid or expired Jwt token"));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
            {
                logger.LogWarning("JWT refresh failed: user not found for ID {UserId}", userId);
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials());
            }

            var oldToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken && t.IsActive);
            if (oldToken is null)
            {
                logger.LogWarning("JWT refresh failed: invalid or expired refresh token for user {Email}", user.Email);
                return Result.Failure<AuthResponse>(UserError.InvalidToken("Invalid or expired refresh token"));
            }

            oldToken.RevokedOn = DateTime.UtcNow;

            var newJwtToken = jwtProvider.GenerateToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken.Token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = newRefreshToken.ExpiresAt,
                UserId = user.Id
            });

            await userManager.UpdateAsync(user);

            logger.LogInformation("Refresh token renewed successfully for user {Email}", user.Email);

            return Result.Success<AuthResponse>(new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                JwtToken: newJwtToken,
                RefreshToken: newRefreshToken
            ));
        }

        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var emailExists = await userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
            if (emailExists)
            {
                logger.LogWarning("Registration failed: email {Email} already exists", request.Email);
                return Result.Failure(UserError.DuplicatedEmail());
            }

            var user = request.Adapt<ApplicationUser>();
            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var description = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                logger.LogError("Registration failed for {Email}: {Errors}", request.Email, description);
                return Result.Failure(ValidationError.InvalidInput(description));
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await SendConfirmationEmail(code, request.Email, user);

            logger.LogInformation("User {Email} registered successfully. Confirmation code generated.", request.Email);

            return Result.Success();
        }

        private static TokenResponse GenerateRefreshToken()
        {
            string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return new TokenResponse(
                Token: token,
                ExpiresAt: DateTime.UtcNow.AddDays(RefreshTokenLifetimeDays)
            );
        }

        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Email confirmation attempt for user {UserId}", request.userId);

            var user = await userManager.FindByIdAsync(request.userId);
            if (user is null)
            {
                logger.LogWarning("Email confirmation failed: invalid user ID {UserId}", request.userId);
                return Result.Failure(UserError.InvalidId());
            }

            if (user.EmailConfirmed)
            {
                logger.LogWarning("Email confirmation failed: user {UserId} already confirmed", request.userId);
                return Result.Failure(UserError.DuplicatedConfirmation());
            }

            string code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.code));
            }
            catch (FormatException)
            {
                logger.LogWarning("Email confirmation failed: invalid confirmation code for user {UserId}", request.userId);
                return Result.Failure(UserError.InvalidCode());
            }

            var result = await userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                var description = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                logger.LogError("Email confirmation failed for user {UserId}: {Errors}", request.userId, description);
                return Result.Failure(ValidationError.InvalidInput(description));
            }

            logger.LogInformation("Email confirmed successfully for user {UserId}", request.userId);
            return Result.Success();
        }

        public async Task<Result> ResendConfirmationEmailAsync(
            ResendConfirmationEmailRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Resending confirmation email to {Email}", request.Email);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                logger.LogWarning("Resend failed: no user found with email {Email}", request.Email);
                return Result.Failure(UserError.InvalidCredentials("No account found with this email."));
            }

            if (user.EmailConfirmed)
            {
                logger.LogWarning("Resend failed: email {Email} already confirmed", request.Email);
                return Result.Failure(UserError.DuplicatedConfirmation("Email is already confirmed."));
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await SendConfirmationEmail(code, request.Email, user);

            logger.LogInformation("Confirmation email re-sent to {Email}", request.Email);

            return Result.Success();
        }

        private async Task SendConfirmationEmail(string code, string confirmationEmail, ApplicationUser user)
        {
            var confirmationLink = $"https://your-frontend.com/confirm-email?userId={user.Id}&code={code}";

            var templateVariables = new Dictionary<string, string>
            {
                { "{{username}}", $"{user.FirstName} {user.LastName}" },
                { "{{confirmation_link}}", confirmationLink }
            };

            await emailService.SendEmailAsync(
                confirmationEmail,
                "Confirm your SurveyBasket account",
                EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", templateVariables));
        }
    }
}