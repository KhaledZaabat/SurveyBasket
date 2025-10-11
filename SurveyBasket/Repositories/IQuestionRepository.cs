namespace SurveyBasket.Repositories;

public interface IQuestionRepository : IScopedService
{
    public Task<Question?> AddAsync(Question question);
    public Task<Question?> GetWithAnswersAsync(int id);
    public Task<bool> IsDuplicateQuestionAsync(string content);
}

