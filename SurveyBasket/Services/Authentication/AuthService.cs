using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Auhtentication_Providers;
using SurveyBasket.Consts;
using SurveyBasket.Shared.Errors;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services.Authentication
{
    public class AuthService(
        IJwtProvider jwtProvider,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
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
            GenerateTokenRequest generateTokenRequest = await GetTokenRequest(user);
            var jwtToken = jwtProvider.GenerateToken(generateTokenRequest);
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
            GenerateTokenRequest generateTokenRequest = await GetTokenRequest(user);
            var newJwtToken = jwtProvider.GenerateToken(generateTokenRequest);
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


            await userManager.AddToRoleAsync(user, DefaultRoles.Member);

            await SendConfirmationEmail(code, request.Email, user);

            logger.LogInformation("User {Email} registered successfully. Confirmation code generated.", request.Email);

            return Result.Success();
        }



        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Email confirmation attempt for user {UserId}", request.userId);

            var user = await userManager.FindByIdAsync(request.userId);
            if (user is null)
            {
                logger.LogWarning("Email confirmation failed: invalid user ID {UserId}", request.userId);
                return Result.Success();
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
            Contracts.Authentication.Requests.ResendConfirmationEmailRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("Resending confirmation email to {Email}", request.Email);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                logger.LogWarning("Resend failed: no user found with email {Email}", request.Email);
                return Result.Success();
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



        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return Result.Success();

            var errors = result.Errors;
            if (result.Errors.Any(e => e.Code.Contains("PasswordMismatch")))
                return Result.Failure(UserError.InvalidCredentials("Current password is incorrect"));

            if (result.Errors.Any(e => e.Code.Contains("PasswordTooShort") || e.Code.Contains("PasswordRequires")))
                return Result.Failure(UserError.InvalidSubmission("New password does not meet requirements"));


            return Result.Failure(UserError.InvalidSubmission("Failed to change password"));


        }
        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserError.InvalidCode());


            var passwordCheck = await userManager.CheckPasswordAsync(user, request.NewPassword);
            if (passwordCheck)
                return Result.Failure(UserError.InvalidSubmission("New password cannot be the same as the current password"));


            string code;
            try
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserError.InvalidCode("Invalid or expired reset token"));
            }


            var result = await userManager.ResetPasswordAsync(user, code, request.NewPassword);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(e => e.Code.Contains("PasswordTooShort") || e.Code.Contains("PasswordRequires")))
                    return Result.Failure(UserError.InvalidSubmission("New password does not meet requirements"));

                return Result.Failure(UserError.InvalidSubmission("Failed to reset password"));
            }

            return Result.Success();
        }


        private async Task<IList<string>> GetAllRoles(ApplicationUser user)
        {

            return await userManager.GetRolesAsync(user);
        }

        private async Task<IList<string>> GetAllPermissionsAsync(ApplicationUser user, IList<string> roles)
        {
            var permissions = new HashSet<string>();



            foreach (var roleName in roles)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role == null) continue;

                var claims = await roleManager.GetClaimsAsync(role);

                foreach (var claim in claims)
                {
                    if (claim.Type == Permissions.Type && !string.IsNullOrEmpty(claim.Value))
                    {
                        permissions.Add(claim.Value);
                    }
                }
            }

            return permissions.ToList();
        }
        private async Task<GenerateTokenRequest> GetTokenRequest(ApplicationUser user)
        {
            IList<string> roles = await GetAllRoles(user);
            IList<string> permissions = await GetAllPermissionsAsync(user, roles);
            GenerateTokenRequest generateTokenRequest = new GenerateTokenRequest(user, roles, permissions);

            return generateTokenRequest;
        }
        private static TokenResponse GenerateRefreshToken()
        {
            string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            return new TokenResponse(
                Token: token,
                ExpiresAt: DateTime.UtcNow.AddDays(RefreshTokenLifetimeDays)
            );
        }
        public async Task<Result> SendForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserError.EmailNotConfirmed());
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));


            await SendResetPasswordEmail(code, user);

            return Result.Success();
        }

        private Task SendConfirmationEmail(string code, string confirmationEmail, ApplicationUser user)
        {
            var confirmationLink = $"https://frontend.com/confirm-email?userId={user.Id}&code={code}";

            var templateVariables = new Dictionary<string, string>
            {
                { "{{username}}", $"{user.FirstName} {user.LastName}" },
                { "{{confirmation_link}}", confirmationLink }
            };

            BackgroundJob.Enqueue(() => emailService.SendEmailAsync(
                confirmationEmail,
                "Confirm your SurveyBasket account",
                HtmlBodyBuilder.GenerateEmailBody("EmailConfirmation", templateVariables)));
            return Task.CompletedTask;
        }


        private Task SendResetPasswordEmail(string code, ApplicationUser user)
        {

            var resetLink = $"https://frontend.com/reset-password?Email={user.Email}&code={code}";


            var templateVariables = new Dictionary<string, string>
    {
        { "{{username}}", $"{user.FirstName} {user.LastName}" },
        { "{{reset_link}}", resetLink }
    };


            BackgroundJob.Enqueue(() => emailService.SendEmailAsync(
                user.Email!,
                "Reset your SurveyBasket password",
                HtmlBodyBuilder.GenerateEmailBody("ForgetPassword", templateVariables)
            ));

            return Task.CompletedTask;
        }

    }
}