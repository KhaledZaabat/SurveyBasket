namespace SurveyBasket.Repositories;

public interface IQuestionRepository : IScopedService
{
    public Task<Question?> AddAsync(Question question, CancellationToken token = default);
    public Task<Question?> GetWithAnswersAsync(int pollId, int questionId, CancellationToken token = default);
    public Task<bool> IsDuplicateQuestionAsync(string content, CancellationToken token = default);
    public Task<ICollection<Question>> GetAllAsync(int poolId, CancellationToken token = default);
}

