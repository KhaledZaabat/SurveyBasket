namespace SurveyBasket.Repositories;

public interface IQuestionRepository : IScopedService
{
    public Task<Question?> AddAsync(Question question, CancellationToken token = default);
    public Task<Question?> GetWithAnswersAsync(int id, CancellationToken token = default);
    public Task<bool> IsDuplicateQuestionAsync(string content, CancellationToken token = default);
}

