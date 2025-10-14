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

    public async Task<ICollection<Survey>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Surveys.AsNoTracking().ToListAsync(cancellationToken);
    public async Task<ICollection<Survey>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default) =>
         await db.Surveys.IgnoreQueryFilters().AsNoTracking().ToListAsync(cancellationToken);

    public Task<Survey?> GetByIdAsync(int surveyId, CancellationToken cancellationToken = default)
        => db.Surveys.FirstOrDefaultAsync(p => p.Id == surveyId, cancellationToken);

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
    public async Task<bool> ExistByIdAsync(int surveyId, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(e => e.Id == surveyId, cancellationToken);
    public async Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default)
           => await db.Surveys.AnyAsync(e => e.Title == title, cancellationToken);
    public async Task<bool> ExistByTitleWithDifferentIdAsync(string title, int id, CancellationToken cancellationToken = default)
     => await db.Surveys.AnyAsync(e => e.Title == title && id != e.Id, cancellationToken);

    public async Task<ICollection<Survey>> GetCurrentSurveysAsync(CancellationToken cancellationToken = default)
        => await db.Surveys
        .AsNoTracking()
        .Where(s => DateOnly.FromDateTime(DateTime.UtcNow) >= s.StartsAt
        && DateOnly.FromDateTime(DateTime.UtcNow) <= s.EndsAt && s.Status.IsPublished).ToListAsync(cancellationToken);

    public async Task<bool> IsSurveyClosed(int surveyId, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(s =>
            s.Id == surveyId &&
            DateOnly.FromDateTime(DateTime.UtcNow) > s.EndsAt &&
            s.Status.IsPublished,
            cancellationToken);

    public async Task<bool> IsSurveyNotStarted(int surveyId, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(s =>
            s.Id == surveyId &&
            DateOnly.FromDateTime(DateTime.UtcNow) < s.StartsAt &&
            s.Status.IsPublished,
            cancellationToken);

    public async Task<bool> IsSurveyAvailable(int surveyId, CancellationToken cancellationToken = default)
    => await db.Surveys.AnyAsync(s =>
        s.Id == surveyId &&
        s.Status.IsPublished &&
        DateOnly.FromDateTime(DateTime.UtcNow) >= s.StartsAt &&
        DateOnly.FromDateTime(DateTime.UtcNow) <= s.EndsAt &&
        !s.IsDeleted,
        cancellationToken);

    public async Task<Survey?> GetByIdAsyncIncludingDeletedAsync(int surveyId, CancellationToken cancellationToken = default)
        => await db.Surveys.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == surveyId, cancellationToken);


}
