namespace SurveyBasket.Contracts.Authentication.Responses;

public record TokenResponse(string JwtToken, DateTime ExpiresAt);
