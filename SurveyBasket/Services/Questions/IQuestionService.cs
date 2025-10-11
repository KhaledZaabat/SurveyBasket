using SurveyBasket.Contracts.Question.Requests;

namespace SurveyBasket.Services.Questions;

public interface IQuestionService : IScopedService
{
    public Task<Result<QuestionResponse>> AddAsync(int pollId, CreateQuestionRequest request, CancellationToken token = default);
    public Task<Result<ICollection<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken token = default);
    public Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken token = default);
}

