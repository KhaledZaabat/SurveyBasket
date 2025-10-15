namespace SurveyBasket.Contracts.Authentication.Validations;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(x => x.code)
            .NotEmpty();



        RuleFor(x => x.code).NotEmpty();


    }
}
