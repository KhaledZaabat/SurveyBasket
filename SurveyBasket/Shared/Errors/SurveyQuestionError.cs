namespace SurveyBasket.Shared.Errors;

public sealed record SurveyQuestionError(int StatusCode, string Code, string Description)
    : Error(StatusCode, Code, Description)
{
    public static SurveyQuestionError SurveyQuestionNotFound(string description = "SurveyQuestion not found")
        => new(StatusCodes.Status404NotFound, "SurveyQuestion.NotFound", description);

    public static SurveyQuestionError SurveyNotFound(string description = "Survey with this id not found")
        => new(StatusCodes.Status404NotFound, "SurveyQuestion.Survey.NotFound", description);

    public static SurveyQuestionError UnauthorizedAccess(string description = "You do not have access to this question")
        => new(StatusCodes.Status403Forbidden, "SurveyQuestion.UnauthorizedAccess", description);

    public static SurveyQuestionError ConflictSurveyQuestion(string description = "A question with this content already exists")
        => new(StatusCodes.Status409Conflict, "SurveyQuestion.Conflict", description);

    //public static SurveyQuestionError CreationFailed(string description = "Failed to create the question")
    //    => new(StatusCodes.Status500InternalServerError, "SurveyQuestion.CreationFailed", description);
}
