using Microsoft.AspNetCore.Identity;
using SurveyBasket.Auhtentication_Providers;
using System.Security.Cryptography;

namespace SurveyBasket.Services.Authentication
{
    public class AuthService(IJwtProvider jwtProvider, UserManager<ApplicationUser> userManager) : IAuthService
    {
        private const int RefreshTokenLifetimeDays = 14;

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null) return null;

            bool isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) return null;

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

            return new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                JwtToken: jwtToken,
                RefreshToken: refreshToken
            );
        }

        public async Task<AuthResponse?> RefreshAsync(RefreshRequest request)
        {
            string? userId = jwtProvider.ValidateToken(request.JwtToken);
            if (userId is null) return null;

            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return null;

            var oldToken = user.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken && t.IsActive);
            if (oldToken is null) return null;

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

            return new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                JwtToken: newJwtToken,
                RefreshToken: newRefreshToken
            );
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors)> RegisterAsync(RegisterRequest request)
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
                return (false, result.Errors);

            // Optionally assign a role in future
            // await userManager.AddToRoleAsync(user, "User");

            return (true, null);
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
