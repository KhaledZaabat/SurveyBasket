namespace SurveyBasket.Repositories;

public interface ISurveyQuestionRepository : IScopedService
{
    public Task<SurveyQuestion?> AddAsync(SurveyQuestion question, CancellationToken token = default);
    public Task<SurveyQuestion?> GetQuestionWithOptionsAsync(int surveyId, int questionId, CancellationToken token = default);
    public Task<bool> IsDuplicateSurveyQuestionAsync(string content, CancellationToken token = default);
    public Task<ICollection<SurveyQuestion>> GetAllAsync(int surveyId, CancellationToken token = default);
    public Task<bool> RestoreSurveyQuestion(int surveyId, int questionId, CancellationToken token = default);
    public Task<bool> ExistByIdAsync(int surveyId, int questionId, CancellationToken token = default);
    public Task<bool> ExistByIdAsyncIgnoredFilter(int surveyId, int questionId, CancellationToken token = default);

    public Task<bool> DeleteSurveyQuestionAsync(int surveyId, int questionId, CancellationToken token = default);

    public Task<bool> UpdateAsync(int surveyId, int questionId, SurveyQuestion updatedSurveyQuestion, CancellationToken token = default);
    public Task<bool> ExistByContentWithDifferentId(int surveyId, int questionId, string content, CancellationToken token = default);
    public Task<ICollection<SurveyQuestion>> GetAvailableQuestionAsync(int surveyId, CancellationToken cancellationToken = default);
}

