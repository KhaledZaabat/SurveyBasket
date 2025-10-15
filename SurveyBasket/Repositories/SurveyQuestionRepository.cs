using Microsoft.Extensions.Caching.Hybrid;

namespace SurveyBasket.Repositories;

public class SurveyQuestionRepository(AppDbContext db, HybridCache cache) : ISurveyQuestionRepository
{
    private const string QuestionCacheKeyPrefix = "Question";
    private const string AvailableQuestionsCacheKeyPrefix = "AvailableQuestions";

    public async Task<SurveyQuestion?> AddAsync(SurveyQuestion question, CancellationToken token = default)
    {
        await db.SurveyQuestions.AddAsync(question, token);
        await db.SaveChangesAsync(token);

        await cache.RemoveAsync($"{AvailableQuestionsCacheKeyPrefix}_{question.SurveyId}", token);

        return question;
    }

    public async Task<SurveyQuestion?> GetQuestionWithOptionsAsync(int surveyId, int questionId, CancellationToken token = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };

        return await cache.GetOrCreateAsync(
            $"{QuestionCacheKeyPrefix}_{surveyId}_{questionId}",
            async cancel => await db.SurveyQuestions
                .Include(q => q.SurveyOptions)
                .FirstOrDefaultAsync(q => q.Id == questionId && q.SurveyId == surveyId, cancel),
            options,
            cancellationToken: token
        );
    }

    public async Task<bool> ExistByIdAsync(int surveyId, int questionId, CancellationToken token = default)
        => await db.SurveyQuestions.AnyAsync(p => p.Id == questionId && p.SurveyId == surveyId, token);

    public async Task<SurveyQuestion?> GetByIdAsync(int surveyId, int questionId, CancellationToken token = default)
        => await db.SurveyQuestions.FirstOrDefaultAsync(q => q.SurveyId == surveyId && q.Id == questionId, token);

    public async Task<bool> ExistByIdAsyncIgnoredFilter(int surveyId, int questionId, CancellationToken token = default)
        => await db.SurveyQuestions.IgnoreQueryFilters().AnyAsync(p => p.Id == questionId && p.SurveyId == surveyId, token);

    public async Task<bool> IsDuplicateSurveyQuestionAsync(string content, CancellationToken token = default)
        => await db.SurveyQuestions.AnyAsync(x => x.Content == content, token);

    public async Task<ICollection<SurveyQuestion>> GetAllAsync(int surveyId, CancellationToken token = default)
        => await db.SurveyQuestions
            .Where(q => q.SurveyId == surveyId)
            .Include(p => p.SurveyOptions)
            .AsNoTracking()
            .ToListAsync(token);

    public async Task<SurveyQuestion?> GetByIdIgnoringDeletionFilterAsync(int surveyId, int questionId, CancellationToken token = default)
        => await db.SurveyQuestions.IgnoreQueryFilters().FirstOrDefaultAsync(q => q.Id == questionId && q.SurveyId == surveyId, token);

    public async Task<bool> UpdateEntityAsync(SurveyQuestion question, CancellationToken token = default)
    {
        db.SurveyQuestions.Update(question);
        await db.SaveChangesAsync(token);

        await cache.RemoveAsync($"{QuestionCacheKeyPrefix}_{question.SurveyId}_{question.Id}", token);
        await cache.RemoveAsync($"{AvailableQuestionsCacheKeyPrefix}_{question.SurveyId}", token);

        return true;
    }

    public async Task<bool> DeleteAsync(SurveyQuestion question, CancellationToken token = default)
    {
        db.Remove(question);
        await db.SaveChangesAsync(token);

        await cache.RemoveAsync($"{QuestionCacheKeyPrefix}_{question.SurveyId}_{question.Id}", token);
        await cache.RemoveAsync($"{AvailableQuestionsCacheKeyPrefix}_{question.SurveyId}", token);

        return true;
    }

    public async Task<bool> UpdateWithOptionsResetAsync(int surveyId, int questionId, SurveyQuestion updatedSurveyQuestion, CancellationToken token = default)
    {
        var foundSurveyQuestion = await db.SurveyQuestions
            .Include(q => q.SurveyOptions)
            .FirstOrDefaultAsync(q => q.SurveyId == surveyId && q.Id == questionId, token);

        if (foundSurveyQuestion == null)
            return false;

        db.SurveyOptions.RemoveRange(await db.SurveyOptions
            .Where(a => a.SurveyQuestionId == questionId).ToArrayAsync(token));

        foundSurveyQuestion.Content = updatedSurveyQuestion.Content;

        foreach (var surveyOption in updatedSurveyQuestion.SurveyOptions)
        {
            db.SurveyOptions.Add(new SurveyOption
            {
                Content = surveyOption.Content,
                SurveyQuestionId = questionId
            });
        }

        await db.SaveChangesAsync(token);

        await cache.RemoveAsync($"{QuestionCacheKeyPrefix}_{surveyId}_{questionId}", token);
        await cache.RemoveAsync($"{AvailableQuestionsCacheKeyPrefix}_{surveyId}", token);

        return true;
    }

    public async Task<bool> ExistByContentWithDifferentId(int surveyId, int questionId, string content, CancellationToken token = default)
        => await db.SurveyQuestions.AnyAsync(q => q.SurveyId == surveyId && q.Id != questionId && q.Content == content, token);

    public async Task<ICollection<SurveyQuestion>> GetAvailableQuestionAsync(int surveyId, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(5)
        };

        return await cache.GetOrCreateAsync(
            $"{AvailableQuestionsCacheKeyPrefix}_{surveyId}",
            async cancel => await db.SurveyQuestions
                .Include(q => q.SurveyOptions)
                .AsNoTracking()
                .Where(q => q.SurveyId == surveyId)
                .ToListAsync(cancel),
            options,
            cancellationToken: cancellationToken
        );
    }
}