

namespace SurveyBasket.Repositories;

public interface IUserSubmissionsRepository : IScopedService
{
    public Task<bool> IsSubmittedBeforeAsync(int surveyId, string userId, CancellationToken token = default);
    public Task<bool> AddAsync(UserSubmission submission, CancellationToken token = default);
    public Task<List<UserSubmission>?> GetSurveySubmissionsAsync(int surveyId, CancellationToken cancellationToken = default);
    public Task<List<(DateOnly submittedOn, int count)>> GetSubmissionsPerDayCountAsync(int surveyId, CancellationToken cancellationToken = default);
    public Task<List<SurveyStatistics>> GetSurveyStatistics(int surveyId, CancellationToken cancellationToken = default);
}

