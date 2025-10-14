namespace SurveyBasket.Contracts.Results;

public record SubmissionResponse(
    string SubmitterName,
    DateTime SubmittedDate,
    IEnumerable<QuestionOptionResponse> SelectedAnswers
);