namespace SurveyBasket.Shared.Erros;

public sealed record DomainError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static DomainError NotFound(string entityName)
        => new(404, $"Domain.{entityName}NotFound", $"{entityName} not found.");

    public static DomainError Conflict(string entityName)
        => new(409, $"Domain.{entityName}Conflict", $"{entityName} already exists.");
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