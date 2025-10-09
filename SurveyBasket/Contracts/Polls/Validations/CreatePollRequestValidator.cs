using SurveyBasket.Contracts.Polls.Requests;

namespace SurveyBasket.Contracts.Polls.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title)
             .NotEmpty()
             .Length(3, 100);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .Length(3, 1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));

        RuleFor(x => x.EndsAt)
            .NotEmpty();

        RuleFor(x => x)
            .Must(HasValidDates)
            .WithName(nameof(CreatePollRequest.EndsAt))
            .WithMessage("{PropertyName} must be greater than or equals start date");
    }

    private bool HasValidDates(CreatePollRequest pollRequest)
    {
        return pollRequest.EndsAt >= pollRequest.StartsAt;
    }


}
