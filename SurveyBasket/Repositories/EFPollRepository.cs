namespace SurveyBasket.Repositories;

public class EFPollRepository(AppDbContext db) : IPollRepository
{

    public async Task<Poll> Add(Poll poll, CancellationToken cancellationToken = default)
    {
        bool exists = await db.Polls
         .AnyAsync(p => p.Title == poll.Title, cancellationToken); // assuming Title is unique

        if (exists)
            return null;

        await db.Polls.AddAsync(poll, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        return poll;
    }


    public Task<int> Count(CancellationToken cancellationToken = default) => db.Polls.CountAsync(cancellationToken);


    public Task<List<Poll>> GetAll(CancellationToken cancellationToken) =>
        db.Polls.AsNoTracking().ToListAsync(cancellationToken);


    public Task<Poll?> GetById(int id, CancellationToken cancellationToken) => db.Polls.FindAsync(id, cancellationToken).AsTask();


    public async Task<bool> Update(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var existingPoll = await db.Polls.FindAsync(new object[] { id }, cancellationToken);

        if (existingPoll is null)
            return false;

        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.EndsAt = poll.EndsAt;
        existingPoll.StartsAt = poll.StartsAt;

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }


    public async Task<bool> Delete(int id, CancellationToken token = default)
    {
        var rowsAffected = await db.Polls
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync(token);
        return rowsAffected > 0;
    }
    public async Task<PublishStatus?> TogglePublish(int id, CancellationToken cancellationToken = default)
    {
        var existingPoll = await db.Polls.FindAsync(new object[] { id }, cancellationToken);

        if (existingPoll is null)
            return null;

        existingPoll.Status = new PublishStatus(!existingPoll.Status.IsPublished);
        await db.SaveChangesAsync(cancellationToken);

        return existingPoll.Status;

    }
}