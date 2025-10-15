namespace SurveyBasket.Shared.Erros;

public sealed record DomainError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static DomainError NotFound(string entityName)
        => new(StatusCodes.Status404NotFound, $"Domain.{entityName}NotFound", $"{entityName} not found.");

    public static DomainError Conflict(string entityName)
        => new(StatusCodes.Status409Conflict, $"Domain.{entityName}Conflict", $"{entityName} already exists.");
}
