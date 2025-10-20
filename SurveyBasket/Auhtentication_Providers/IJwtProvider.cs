namespace SurveyBasket.Auhtentication_Providers
{
    public interface IJwtProvider : ISingletonService
    {
        public TokenResponse GenerateToken(GenerateTokenRequest request);
        public string? ValidateToken(string token);
    }
}
