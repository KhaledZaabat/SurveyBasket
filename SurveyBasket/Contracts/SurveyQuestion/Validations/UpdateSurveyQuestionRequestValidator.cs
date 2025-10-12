using SurveyBasket.Contracts.SurveyQuestion.Requests;

namespace SurveyBasket.Contracts.SurveyQuestion.Validations;

public class UpdateSurveySurveyQuestionRequestValidator : AbstractValidator<UpdateSurveyQuestionRequest>
{
    public UpdateSurveySurveyQuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);


        RuleFor(x => x.SurveyQuestions)
            .NotNull()
            .WithMessage("SurveyOptions collection cannot be null.");


        RuleFor(x => x.SurveyQuestions)
            .Must(a => a.Count >= 2)
            .WithMessage("A question must have at least two surveyOptions.")
            .When(x => x.SurveyQuestions != null);


        RuleFor(x => x.SurveyQuestions)
            .Must(a => a
                .Select(ans => ans.Trim().ToLower())
                .Distinct()
                .Count() == a.Count)
            .WithMessage("Duplicate surveyOptions are not allowed.")
            .When(x => x.SurveyQuestions != null);


    }
}