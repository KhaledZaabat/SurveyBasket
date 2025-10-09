namespace SurveyBasket.Contracts.Authentication.Requests;

public record RefreshRequest(string JwtToken, string RefreshToken);




