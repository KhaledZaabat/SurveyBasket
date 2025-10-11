namespace SurveyBasket.Shared.Errors;

public sealed record QuestionError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static QuestionError QuestionNotFound(string description = "Question not found")
        => new(StatusCodes.Status404NotFound, "Question.NotFound", description);

    public static QuestionError PoolNotFound(string description = "Poll with this id not found")
        => new(StatusCodes.Status404NotFound, "Question.Pool.NotFound", description);

    public static QuestionError UnauthorizedAccess(string description = "You do not have access to this question")
        => new(StatusCodes.Status403Forbidden, "Question.UnauthorizedAccess", description);

    public static QuestionError ConflictQuestion(string description = "A question with this content already exists")
        => new(StatusCodes.Status409Conflict, "Question.Conflict", description);

    public static QuestionError CreationFailed(string description = "Failed to create the question")
        => new(StatusCodes.Status500InternalServerError, "Question.CreationFailed", description);
}
