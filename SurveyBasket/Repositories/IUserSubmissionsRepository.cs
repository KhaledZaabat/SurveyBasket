namespace SurveyBasket.Repositories;

public interface IUserSubmissionsRepository : IScopedService
{
    public Task<bool> SubmittedBeforeAsync(int surveyId, string userId, CancellationToken token = default);
    public Task<bool> AddAsync(UserSubmission submission, CancellationToken token = default);

}

