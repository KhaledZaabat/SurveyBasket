namespace SurveyBasket.Contracts.Authentication.Requests;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
