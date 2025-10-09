using Microsoft.AspNetCore.Identity;
using SurveyBasket.Auhtentication_Providers;

namespace SurveyBasket.Services.Authentication
{
    public class AuthService(IJwtProvider _jwtProvider, UserManager<ApplicationUser> _userManager) : IAuthService
    {
        public async Task<AuthResponse?> Login(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            bool success = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!success) return null;

            var token = _jwtProvider.GenerateToken(user);

            return new AuthResponse(
                Id: user.Id,
                Email: user.Email!,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Token: token
            );
        }

        public async Task<(bool succeeded, IEnumerable<IdentityError>? errors)> Register(RegisterRequest request)
        {
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return (false, result.Errors);

            // Optional: assign role (if role-based registration is added later)
            // await _userManager.AddToRoleAsync(user, request.Role);

            return (true, null);
        }
    }
}
