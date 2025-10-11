namespace SurveyBasket.Repositories;

public interface IPollRepository : IScopedService
{
    Task<Poll?> AddAsync(Poll poll, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<List<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Poll?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Poll poll, CancellationToken cancellationToken = default);
    Task DeleteAsync(Poll poll, CancellationToken cancellationToken = default);
    public Task<bool> ExistByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default);
    public Task<bool> ExistByTitleWithDifferentIdAsync(string title, int id, CancellationToken cancellationToken = default);
}
