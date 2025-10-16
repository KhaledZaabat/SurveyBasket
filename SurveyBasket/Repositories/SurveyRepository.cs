using Microsoft.Extensions.Caching.Hybrid;

namespace SurveyBasket.Repositories;

public class SurveyRepository(AppDbContext db, HybridCache cache, ILogger<SurveyRepository> logger) : ISurveyRepository
{
    private const string CurrentSurveysCacheKey = "CurrentSurveys";

    private static string GetSurveyCacheKey(int surveyId) => $"Survey_{surveyId}";

    public async Task<Survey?> AddAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        await db.Surveys.AddAsync(survey, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(CurrentSurveysCacheKey, cancellationToken);

        return survey;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
        => db.Surveys.CountAsync(cancellationToken);

    public async Task<ICollection<Survey>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Surveys.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<ICollection<Survey>> GetAllIncludingDeletedAsync(CancellationToken cancellationToken = default)
        => await db.Surveys.IgnoreQueryFilters().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Survey?> GetByIdAsync(int surveyId, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };

        return await cache.GetOrCreateAsync(
            GetSurveyCacheKey(surveyId),
            async cancel => await db.Surveys.FirstOrDefaultAsync(p => p.Id == surveyId, cancel),
            options,
            cancellationToken: cancellationToken
        );
    }

    public async Task UpdateAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        db.Surveys.Update(survey);
        await db.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(GetSurveyCacheKey(survey.Id), cancellationToken);
        await cache.RemoveAsync(CurrentSurveysCacheKey, cancellationToken);
    }

    public async Task DeleteAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        db.Surveys.Remove(survey);
        await db.SaveChangesAsync(cancellationToken);

        await cache.RemoveAsync(GetSurveyCacheKey(survey.Id), cancellationToken);
        await cache.RemoveAsync(CurrentSurveysCacheKey, cancellationToken);
    }

    public async Task<bool> ExistByIdAsync(int surveyId, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(e => e.Id == surveyId, cancellationToken);

    public async Task<bool> ExistByTitleAsync(string title, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(e => e.Title == title, cancellationToken);

    public async Task<bool> ExistByTitleWithDifferentIdAsync(string title, int id, CancellationToken cancellationToken = default)
        => await db.Surveys.AnyAsync(e => e.Title == title && id != e.Id, cancellationToken);

    public async Task<ICollection<Survey>> GetCurrentSurveysAsync(CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5),
            LocalCacheExpiration = TimeSpan.FromMinutes(2)
        };

        return await cache.GetOrCreateAsync(
            CurrentSurveysCacheKey,
            async cancel => await db.Surveys
                .AsNoTracking()
                .Where(s => DateOnly.FromDateTime(DateTime.UtcNow) >= s.StartsAt
                            && DateOnly.FromDateTime(DateTime.UtcNow) <= s.EndsAt
                            && s.Status.IsPublished)
                .ToListAsync(cancel),
            options,
            cancellationToken: cancellationToken
        );
    }

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


    public async Task<ICollection<Survey>> GetPublishedTodaysSurveys(CancellationToken cancellationToken = default)
        => await db.Surveys.AsNoTracking()
        .Where(s => DateOnly.FromDateTime(DateTime.UtcNow) == s.StartsAt && s.Status.IsPublished)
        .ToListAsync(cancellationToken);

}
