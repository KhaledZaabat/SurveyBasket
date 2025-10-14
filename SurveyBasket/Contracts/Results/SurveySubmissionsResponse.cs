namespace SurveyBasket.Contracts.Results;

public record SurveySubmissionsResponse(
    string Title,
    IEnumerable<SubmissionResponse> Submissions
);
// we want all the submissions for specific survey