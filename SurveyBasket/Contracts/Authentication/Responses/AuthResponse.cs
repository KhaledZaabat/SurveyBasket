namespace SurveyBasket.Contracts.Authentication.Responses;

public record AuthResponse(
    string Id,
    string? Email,
    string FirstName,
    string LastName,
    TokenResponse JwtToken,
    TokenResponse RefreshToken
);