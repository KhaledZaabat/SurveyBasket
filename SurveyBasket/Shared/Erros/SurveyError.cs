namespace SurveyBasket.Shared.Erros;

public sealed record SurveyError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static SurveyError NotFound(string description = "Survey not found")
        => new(StatusCodes.Status404NotFound, "Survey.NotFound", description);

    public static SurveyError AlreadyClosed(string description = "Survey has been closed")
        => new(StatusCodes.Status400BadRequest, "Survey.AlreadyClosed", description);

    public static SurveyError UnauthorizedAccess(string description = "You do not have access to this survey")
        => new(StatusCodes.Status403Forbidden, "Survey.UnauthorizedAccess", description);
    public static SurveyError Conflict(string description = "A survey with this title already exists")
      => new(StatusCodes.Status409Conflict, "Survey.Conflict", description);
    public static SurveyError NotOpened(string description = "Survey has not been opened yet")
    => new(StatusCodes.Status400BadRequest, "Survey.NotOpened", description);
}
