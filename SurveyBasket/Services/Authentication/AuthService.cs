using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Auhtentication_Providers;
using SurveyBasket.Shared.Errors;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services.Authentication
{
    public class AuthService(IJwtProvider jwtProvider
        , UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : IAuthService
    {
        private const int RefreshTokenLifetimeDays = 14;

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return Result.Failure<AuthResponse>(UserError.InvalidCredentials());
            }

            if (!user.EmailConfirmed)
                return Result.Failure<AuthResponse>(UserError.EmailNotConfirmed());
            var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (!result.Succeeded)
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

        public async Task<Result<AuthResponse>> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
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

        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)


        {



            var emailIsExists = await userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

            if (emailIsExists)
                return Result.Failure(UserError.DuplicatedEmail());
            var user = request.Adapt<ApplicationUser>();

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {

                var description = string.Join($"{Environment.NewLine}", result.Errors.Select(e => e.Description));
                return Result.Failure(ValidationError.InvalidInput(description));
            }

            // Optionally assign a role in future
            // await userManager.AddToRoleAsync(user, "User");
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            Console.WriteLine(code);
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
            var user = await userManager.FindByIdAsync(request.userId);
            if (user is null)
                return Result.Failure(UserError.InvalidId());

            if (user.EmailConfirmed)
                return Result.Failure(UserError.DuplicatedConfirmation());

            string code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserError.InvalidCode());
            }

            var result = await userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                var description = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return Result.Failure(ValidationError.InvalidInput(description));
            }

            return Result.Success();
        }



        public async Task<Result> ResendConfirmationEmailAsync(
         ResendConfirmationEmailRequest request,
          CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result.Failure(UserError.InvalidCredentials("No account found with this email."));

            if (user.EmailConfirmed)
                return Result.Failure(UserError.DuplicatedConfirmation("Email is already confirmed."));

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            Console.WriteLine(code);
            // Normally, send the confirmation email here
            // Example: await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Your code: {code}");

            return Result.Success();
        }

    }
}
