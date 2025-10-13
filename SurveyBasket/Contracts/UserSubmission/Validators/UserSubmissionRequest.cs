using SurveyBasket.Contracts.SubmissionDetails.Requests;

namespace SurveyBasket.Contracts.UserSubmission.Validators;

public record UserSubmissionRequest(
    ICollection<SubmissionDetailRequest> submissionDetails);
