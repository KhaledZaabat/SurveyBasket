namespace SurveyBasket.Repositories;

public class EFPollRepository(AppDbContext db) : IPollRepository
{
    public async Task<Poll?> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await db.Polls.AddAsync(poll, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => db.Polls.CountAsync(cancellationToken);

    public Task<List<Poll>> GetAllAsync(CancellationToken cancellationToken = default)
        => db.Polls.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Poll?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => db.Polls.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task UpdateAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        db.Polls.Update(poll);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        db.Polls.Remove(poll);
        await db.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> ExistByIdAsync(int id, CancellationToken cancellationToken = default)
        => await db.Polls.AnyAsync(e => e.Id == id, cancellationToken);
    public async Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default)
           => await db.Polls.AnyAsync(e => e.Title == title, cancellationToken);
    public async Task<bool> ExistByTitleWithDifferentIdAsync(string title, int id, CancellationToken cancellationToken = default)
     => await db.Polls.AnyAsync(e => e.Title == title && id != e.Id, cancellationToken);


}
