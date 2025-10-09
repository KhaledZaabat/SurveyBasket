namespace SurveyBasket.Auhtentication_Providers
{
    public interface IJwtProvider : ISingletonService
    {
        public TokenResponse GenerateToken(ApplicationUser user);
        public string? ValidateToken(string token);
    }
}
