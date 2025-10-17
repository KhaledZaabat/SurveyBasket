namespace SurveyBasket.Contracts.Users.Requests;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);