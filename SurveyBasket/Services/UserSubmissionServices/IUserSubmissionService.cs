namespace SurveyBasket.Services.UserSubmissionServices;

public interface IUserSubmissionService : IScopedService
{
    Task<Result> AddAsync(int surveyId, string userId, UserSubmissionRequest request, CancellationToken cancellationToken = default);
}
