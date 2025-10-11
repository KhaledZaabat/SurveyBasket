using SurveyBasket.Contracts.Question.Requests;

namespace SurveyBasket.Contracts.Question.Validations;

public class QuestionRequestValidator : AbstractValidator<CreateQuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);


        RuleFor(x => x.Answers)
            .NotNull()
            .WithMessage("Answers collection cannot be null.");


        RuleFor(x => x.Answers)
            .Must(a => a.Count >= 2)
            .WithMessage("A question must have at least two answers.")
            .When(x => x.Answers != null);


        RuleFor(x => x.Answers)
            .Must(a => a
                .Select(ans => ans.Trim().ToLower())
                .Distinct()
                .Count() == a.Count)
            .WithMessage("Duplicate answers are not allowed.")
            .When(x => x.Answers != null);


    }
}
