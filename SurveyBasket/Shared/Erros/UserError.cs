namespace SurveyBasket.Shared.Erros;

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

}

//public sealed record Error(int StatusCode, string Description)
//{
//    public static readonly Error None = new(200, string.Empty);

//    public static Error InvalidToken(string description = "Invalid or expired token") => new(401, "Invalid or expired token");
//    public static Error InvalidCredentials(string description = "Invalid email or password") => new(401, description);
//    public static Error NotFound(string description = "Not found") => new(404, description);
//    public static Error BadRequest(string description = "Bad request") => new(400, description);
//    public static Error Unauthorized(string description = "Unauthorized") => new(401, description);
//    public static Error Forbidden(string description = "Forbidden") => new(403, description);
//    public static Error Conflict(string description = "Conflict") => new(409, description);
//    public static Error Validation(string description = "Validation error") => new(422, description);
//    public static Error Failure(string description = "Internal server error") => new(500, description);
//    public override string ToString() => $"{StatusCode}: {Description}";
//}