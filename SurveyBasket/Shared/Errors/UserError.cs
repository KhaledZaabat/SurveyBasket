namespace SurveyBasket.Shared.Errors;


public sealed record UserError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static UserError InvalidToken(string description = "Invalid or expired token")
        => new(StatusCodes.Status401Unauthorized, "User.InvalidToken", description);

    public static UserError InvalidCredentials(string description = "Invalid email or password")
        => new(StatusCodes.Status401Unauthorized, "User.InvalidCredentials", description);

    public static UserError Unauthorized(string description = "Unauthorized")
        => new(StatusCodes.Status401Unauthorized, "User.Unauthorized", description);

    public static UserError Forbidden(string description = "Forbidden")
        => new(StatusCodes.Status403Forbidden, "User.Forbidden", description);

    public static UserError Conflict(string description = "Conflict")
        => new(StatusCodes.Status409Conflict, "User.Conflict", description);

    public static UserError InvalidSubmission(string description = "Invalid submission data")
        => new(StatusCodes.Status400BadRequest, "User.InvalidSubmission", description);

    public static UserError EmailNotConfirmed(string description = "Email is not confirmed")
        => new(StatusCodes.Status401Unauthorized, "User.EmailNotConfirmed", description);
    public static UserError DuplicatedEmail(string description = "Another user with the same email already exists")
    => new(StatusCodes.Status409Conflict, "User.DuplicatedEmail", description);
    public static UserError DuplicatedConfirmation(string description = "User Already Confirmed the email")
=> new(StatusCodes.Status409Conflict, "User.DuplicatedConfirmation", description);
    public static UserError InvalidId(string description = "Invalid User Id")
   => new(StatusCodes.Status409Conflict, "User.InvalidId", description);

    public static UserError InvalidCode(string description = "Invalid Confirmation Code")
=> new(StatusCodes.Status409Conflict, "User.InvalidCode", description);
}
