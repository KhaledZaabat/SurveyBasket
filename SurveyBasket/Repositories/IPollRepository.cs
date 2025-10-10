namespace SurveyBasket.Repositories;

public interface IPollRepository : IScopedService
{

    public Task<List<Poll>> GetAll(CancellationToken token = default);
    public Task<Poll?> GetById(int id, CancellationToken token = default);
    public Task<Poll?> Add(Poll poll, CancellationToken token = default);
    public Task<bool> Update(int id, Poll poll, CancellationToken token = default);
    public Task<bool> Delete(int id, CancellationToken token = default);
    public Task<int> Count(CancellationToken token = default);
    Task<PublishStatus> TogglePublish(int id, CancellationToken cancellationToken = default);

}
