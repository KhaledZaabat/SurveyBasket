namespace SurveyBasket.Contracts.Authentication.Requests;

public sealed record ResetPasswordRequest
{

    public required string Email { get; init; }

    public required string Code { get; init; }


    public required string NewPassword { get; init; }
}
