namespace SurveyBasket.Contracts.Authentication.Requests;



public record GenerateTokenRequest(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);


