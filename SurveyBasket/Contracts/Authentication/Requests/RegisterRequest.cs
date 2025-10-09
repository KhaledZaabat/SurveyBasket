namespace SurveyBasket.Contracts.Authentication.Requests;

public record RegisterRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName
// string role later
);
