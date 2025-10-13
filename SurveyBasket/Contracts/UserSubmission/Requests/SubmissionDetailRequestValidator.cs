using SurveyBasket.Contracts.SubmissionDetails.Requests;

namespace SurveyBasket.Contracts.UserSubmission.Requests;

public class SubmissionDetailRequestValidator : AbstractValidator<SubmissionDetailRequest>
{
    public SubmissionDetailRequestValidator()
    {
        RuleFor(x => x.QuestionId)
            .GreaterThan(0);

        RuleFor(x => x.OptionId)
            .GreaterThan(0);
    }
}
