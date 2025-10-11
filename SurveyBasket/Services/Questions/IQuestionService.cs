using SurveyBasket.Contracts.Question.Requests;

namespace SurveyBasket.Services.Questions;

public interface IQuestionService : IScopedService
{
    public Task<Result<QuestionResponse>> AddAsync(int id, CreateQuestionRequest request);
}

