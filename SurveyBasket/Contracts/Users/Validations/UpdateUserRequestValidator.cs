namespace SurveyBasket.Contracts.Authentication.Validations;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserProfileRequest>
{
    public UpdateUserRequestValidator()
    {

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100);
    }
}
