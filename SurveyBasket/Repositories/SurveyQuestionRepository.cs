namespace SurveyBasket.Repositories;

public class SurveyQuestionRepository(AppDbContext db) : ISurveyQuestionRepository
{
    public async Task<SurveyQuestion?> AddAsync(SurveyQuestion question, CancellationToken token = default)
    {
        await db.SurveyQuestions.AddAsync(question, token);
        await db.SaveChangesAsync(token);
        return question;
    }

    public async Task<SurveyQuestion?> GetQuestionWithOptionsAsync(int surveyId, int questionId, CancellationToken token = default)
    {
        return await db.SurveyQuestions
            .Include(q => q.SurveyOptions)
            .FirstOrDefaultAsync(q => (q.Id == questionId) && (q.SurveyId == surveyId), token);
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
        => await db.SurveyQuestions.Where(q => q.SurveyId == surveyId).Include(p => p.SurveyOptions).AsNoTracking().ToListAsync(token);

    public async Task<SurveyQuestion?> GetByIdIgnoringDeletionFilterAsync(int surveyId, int questionId, CancellationToken token = default)
      => await db.SurveyQuestions.IgnoreQueryFilters().FirstOrDefaultAsync(q => q.Id == questionId && q.SurveyId == surveyId, token);






    public async Task<bool> UpdateEntityAsync(SurveyQuestion question, CancellationToken token = default)
    {
        db.SurveyQuestions.Update(question);
        await db.SaveChangesAsync(token);
        return true;
    }
    public async Task<bool> DeleteAsync(SurveyQuestion question, CancellationToken token = default)
    {


        db.Remove(question);

        await db.SaveChangesAsync(token);
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
            .Where(a => a.SurveyQuestionId == questionId).ToArrayAsync()
           );



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



        return true;
    }
    public async Task<bool> ExistByContentWithDifferentId(int surveyId, int questionId, string content, CancellationToken token = default)
        => await db.SurveyQuestions.AnyAsync(q => q.SurveyId == surveyId && q.Id != questionId && q.Content == content);

    public async Task<ICollection<SurveyQuestion>> GetAvailableQuestionAsync(int surveyId, CancellationToken cancellationToken = default)

   => await db.SurveyQuestions.Include(q => q.SurveyOptions).AsNoTracking().Where(q => q.SurveyId == surveyId).ToListAsync(cancellationToken);



}

