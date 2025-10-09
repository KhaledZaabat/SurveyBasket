using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Auhtentication_Providers;
//UserManager<ApplicationUser> _userManager later add it !
public class JwtProvider(IOptions<JwtSettings> jwtOptions) : IJwtProvider
{

    TokenResponse IJwtProvider.GenerateToken(ApplicationUser user)
    {
        JwtSettings _jwtSettings = jwtOptions.Value;
        var issuedAt = DateTime.UtcNow;
        var expiresAt = issuedAt.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);


        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add user roles as claims
        //var roles = await _userManager.GetRolesAsync(user);
        //claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = expiresAt,
            IssuedAt = issuedAt,
            SigningCredentials = creds
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return new TokenResponse(
            Token: handler.WriteToken(token),
            ExpiresAt: expiresAt
        );
    }
    public string? ValidateToken(string token)
    {
        JwtSettings _jwtSettings = jwtOptions.Value;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        try
        {

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,

                ValidateLifetime = true, // Reject expired tokens
                ClockSkew = TimeSpan.Zero // No grace period
            };

            // Validate the token and get the validated security token
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
                return null;

            //  verify the algorithm if needed
            if (jwtToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                return null;

            // Extract and return the Subject ("sub") claim — typically the User ID
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
        }
        catch
        {
            // If validation fails for any reason (expired, tampered, etc.)
            return null;
        }
    }

}