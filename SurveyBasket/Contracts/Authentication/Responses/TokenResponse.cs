namespace SurveyBasket.Contracts.Authentication.Responses;

public record TokenResponse(string Token, DateTime ExpiresAt);
