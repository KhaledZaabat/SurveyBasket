using Microsoft.AspNetCore.Identity;
using SurveyBasket.Auhtentication_Providers;
using System.Security.Cryptography;

namespace SurveyBasket.Services.Authentication
{
    public class AuthService(IJwtProvider jwtProvider, UserManager<ApplicationUser> userManager) : IAuthService
    {
        private const int RefreshTokenLifetimeDays = 14;

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            {
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

            return Result<AuthResponse>.Success<AuthResponse>(new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                JwtToken: jwtToken,
                RefreshToken: refreshToken
            ));
        }

        public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request)
        {
            string? userId = jwtProvider.ValidateToken(request.JwtToken);
            if (userId is null) return Result.Failure<AuthResponse>(UserError.InvalidToken("Invalid or expired Jwt token"));

            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return Result.Failure<AuthResponse>(UserError.InvalidCredentials());
            var oldToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken && t.IsActive);
            if (oldToken is null) return Result.Failure<AuthResponse>(UserError.InvalidToken("Invalid or expired refresh token"));

            // Revoke the old refresh token
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

            return Result.Success<AuthResponse>(new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                JwtToken: newJwtToken,
                RefreshToken: newRefreshToken
            ));
        }

        public async Task<Result> RegisterAsync(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email
            };

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {

                var description = string.Join($"{Environment.NewLine}", result.Errors.Select(e => e.Description));
                return Result.Failure(ValidationError.InvalidInput(description));
            }

            // Optionally assign a role in future
            // await userManager.AddToRoleAsync(user, "User");

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
    }
}
