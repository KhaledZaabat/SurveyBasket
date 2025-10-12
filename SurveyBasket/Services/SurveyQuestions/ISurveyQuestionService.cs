

namespace SurveyBasket.Services.SurveyQuestions;

public interface ISurveyQuestionService : IScopedService
{
    public Task<Result<SurveyQuestionResponse>> AddAsync(int surveyId, CreateSurveyQuestionRequest request, CancellationToken token = default);
    public Task<Result<ICollection<SurveyQuestionResponse>>> GetAllAsync(int surveyId, CancellationToken token = default);
    public Task<Result<SurveyQuestionResponse>> GetByIdAsync(int surveyId, int questionId, CancellationToken token = default);
    public Task<Result> RestoreSurveyQuestion(int surveyId, int questionId, CancellationToken token = default);
    public Task<Result> DeleteSurveyQuestionAsync(int surveyId, int questionId, CancellationToken token = default);
    public Task<Result> UpdateSurveyQuestionAsync(int surveyId, int questionId, UpdateSurveyQuestionRequest updateRequest, CancellationToken token = default);
}

