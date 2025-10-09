namespace SurveyBasket.Repositories;

public class EFPollRepository(AppDbContext db) : IPollRepository
{

    public async Task<Poll> Add(Poll poll, CancellationToken token = default)
    {
        await db.Polls.AddAsync(poll, token);
        await db.SaveChangesAsync(token);
        return poll;
    }


    public Task<int> Count(CancellationToken token) => db.Polls.CountAsync(token = default);


    public Task<List<Poll>> GetAll(CancellationToken token) =>
        db.Polls.AsNoTracking().ToListAsync(token);


    public Task<Poll?> GetById(int id, CancellationToken token) => db.Polls.FindAsync(id, token).AsTask();


    public async Task<bool> Update(int id, Poll poll, CancellationToken token = default)
    {
        var rowsAffected = await db.Polls
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.Title, poll.Title)
                .SetProperty(p => p.Summary, poll.Summary)
                .SetProperty(p => p.EndsAt, poll.EndsAt)
                .SetProperty(p => p.StartsAt, poll.StartsAt), token);
        return rowsAffected > 0;
    }


    public async Task<bool> Delete(int id, CancellationToken token = default)
    {
        var rowsAffected = await db.Polls
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(token);
        return rowsAffected > 0;
    }
    public async Task<bool> TogglePublish(int id, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await db.Polls
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.IsPublished, p => !p.IsPublished),
                cancellationToken);
        return rowsAffected > 0;
    }
}