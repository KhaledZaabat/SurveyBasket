namespace SurveyBasket.Shared.Erros;

public sealed record PollError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static PollError NotFound(string description = "Poll not found")
        => new(404, "Poll.NotFound", description);

    public static PollError AlreadyClosed(string description = "Poll has been closed")
        => new(400, "Poll.AlreadyClosed", description);

    public static PollError UnauthorizedAccess(string description = "You do not have access to this poll")
        => new(403, "Poll.UnauthorizedAccess", description);
    public static PollError Conflict(string description = "A poll with this title already exists")
      => new(403, "Poll.Conflict", description);
}
