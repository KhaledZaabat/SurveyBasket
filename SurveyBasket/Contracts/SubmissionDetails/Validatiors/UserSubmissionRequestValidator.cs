using SurveyBasket.Contracts.UserSubmission.Requests;
using SurveyBasket.Contracts.UserSubmission.Validators;

namespace SurveyBasket.Contracts.SubmissionDetails.Validatiors;

public class UserSubmissionRequestValidator : AbstractValidator<UserSubmissionRequest>
{
    public UserSubmissionRequestValidator()
    {
        RuleFor(x => x.submissionDetails)
            .NotEmpty();

        RuleForEach(x => x.submissionDetails)
           .SetValidator(new SubmissionDetailRequestValidator());
    }
}