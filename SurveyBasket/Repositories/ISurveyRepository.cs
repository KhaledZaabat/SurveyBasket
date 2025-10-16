namespace SurveyBasket.Repositories;

public interface ISurveyRepository : IScopedService
{
    Task<Survey?> AddAsync(Survey survey, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Survey>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ICollection<Survey>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default);

    Task<Survey?> GetByIdAsync(int surveyId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Survey survey, CancellationToken cancellationToken = default);
    Task DeleteAsync(Survey survey, CancellationToken cancellationToken = default);
    public Task<bool> ExistByIdAsync(int surveyId, CancellationToken cancellationToken = default);
    public Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default);
    public Task<bool> ExistByTitleWithDifferentIdAsync(string title, int surveyId, CancellationToken cancellationToken = default);
    public Task<ICollection<Survey>> GetCurrentSurveysAsync(CancellationToken cancellationToken = default);
    public Task<bool> IsSurveyClosed(int surveyId, CancellationToken cancellationToken = default);

    public Task<bool> IsSurveyNotStarted(int surveyId, CancellationToken cancellationToken = default);
    public Task<bool> IsSurveyAvailable(int surveyId, CancellationToken cancellationToken = default);
    public Task<Survey?> GetByIdAsyncIncludingDeletedAsync(int surveyId, CancellationToken cancellationToken = default);
    public Task<ICollection<Survey>> GetPublishedTodaysSurveys(CancellationToken cancellationToken = default);


}
