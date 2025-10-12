namespace SurveyBasket.Repositories;

public class EFSurveyRepository(AppDbContext db) : ISurveyRepository
{
    public async Task<Survey?> AddAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        await db.Surveys.AddAsync(survey, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return survey;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => db.Surveys.CountAsync(cancellationToken);

    public Task<List<Survey>> GetAllAsync(CancellationToken cancellationToken = default)
        => db.Surveys.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Survey?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => db.Surveys.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task UpdateAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        db.Surveys.Update(survey);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        db.Surveys.Remove(survey);
        await db.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> ExistByIdAsync(int id, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(e => e.Id == id, cancellationToken);
    public async Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default)
           => await db.Surveys.AnyAsync(e => e.Title == title, cancellationToken);
    public async Task<bool> ExistByTitleWithDifferentIdAsync(string title, int id, CancellationToken cancellationToken = default)
     => await db.Surveys.AnyAsync(e => e.Title == title && id != e.Id, cancellationToken);


}
