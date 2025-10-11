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

    public async Task<UpdateResult> Update(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var existingPoll = await db.Polls.FindAsync(new object[] { id }, cancellationToken);
        if (existingPoll is null)
            return UpdateResult.NotFound;

        bool titleConflict = await db.Polls
            .AnyAsync(p => p.Title == poll.Title && p.Id != id, cancellationToken);

        if (titleConflict)
            return UpdateResult.Conflict;

        existingPoll.Title = poll.Title;
        existingPoll.Summary = poll.Summary;
        existingPoll.EndsAt = poll.EndsAt;
        existingPoll.StartsAt = poll.StartsAt;

        await db.SaveChangesAsync(cancellationToken);
        return UpdateResult.Success;
    }


    public async Task<bool> Delete(int id, CancellationToken cancellationToken = default)
    {
        Poll? existingPoll = await db.Polls.FindAsync(new object[] { id }, cancellationToken);
        if (existingPoll is null) return false;


        db.Polls.Remove(existingPoll);
        await db.SaveChangesAsync(cancellationToken);

        return true;
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
