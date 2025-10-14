

namespace SurveyBasket.Services.Results;

public interface IResultService : IScopedService

{
    public Task<Result<SurveySubmissionsResponse>> GetSurveySubmissionsAsync(int surveyId, CancellationToken cancellationToken = default);
    public Task<Result<List<SubmissionsPerDayResponse>>> GetSubmissionsPerDayCountAsync(int surveyId, CancellationToken cancellationToken = default);
    public Task<Result<List<SurveyStatistics>>> GetSurveyStatistics(int surveyId, CancellationToken cancellationToken = default);
    //Task<Result<IEnumerable<SubmissionsPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
    //Task<Result<IEnumerable<SurveyStatistics>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
}