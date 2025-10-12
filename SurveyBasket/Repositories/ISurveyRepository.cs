namespace SurveyBasket.Repositories;

public interface ISurveyRepository : IScopedService
{
    Task<Survey?> AddAsync(Survey survey, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<List<Survey>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Survey?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Survey survey, CancellationToken cancellationToken = default);
    Task DeleteAsync(Survey survey, CancellationToken cancellationToken = default);
    public Task<bool> ExistByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default);
    public Task<bool> ExistByTitleWithDifferentIdAsync(string title, int id, CancellationToken cancellationToken = default);
}
