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


    public async Task<bool> ExistByIdAsync(int surveyId, int questionId, CancellationToken token = default) => await db.SurveyQuestions.AnyAsync(p => p.Id == questionId && p.SurveyId == surveyId, token);
    public async Task<bool> ExistByIdAsyncIgnoredFilter(int surveyId, int questionId, CancellationToken token = default) => await db.SurveyQuestions.IgnoreQueryFilters().AnyAsync(p => p.Id == questionId && p.SurveyId == surveyId, token);

    public async Task<bool> IsDuplicateSurveyQuestionAsync(string content, CancellationToken token = default)
    {
        return await db.SurveyQuestions.AnyAsync(x => x.Content == content, token);
    }
    public async Task<ICollection<SurveyQuestion>> GetAllAsync(int surveyId, CancellationToken token = default) => await db.SurveyQuestions.Where(q => q.SurveyId == surveyId).Include(p => p.SurveyOptions).AsNoTracking().ToListAsync(token);

    public async Task<bool> RestoreSurveyQuestion(int surveyId, int questionId, CancellationToken token = default)
    {

        var question = await db.SurveyQuestions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(q => q.Id == questionId && q.SurveyId == surveyId, token);

        if (question == null)
            return false;

        question.IsDeleted = false;
        question.DeletedBy = null;
        question.DeletedOn = null;

        await db.SaveChangesAsync(token);
        return true;
    }
    public async Task<bool> DeleteSurveyQuestionAsync(int surveyId, int questionId, CancellationToken token = default)
    {

        SurveyQuestion? question = await db.SurveyQuestions
            .FirstOrDefaultAsync(q => q.Id == questionId && q.SurveyId == surveyId, token);

        if (question == null)
            return false;
        db.Remove(question);

        await db.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> UpdateAsync(int surveyId, int questionId, SurveyQuestion updatedSurveyQuestion, CancellationToken token = default)
    {

        var foundSurveyQuestion = await db.SurveyQuestions
            .Include(q => q.SurveyOptions)
            .FirstOrDefaultAsync(q => q.SurveyId == surveyId && q.Id == questionId, token);

        if (foundSurveyQuestion == null)
            return false;




        await db.SurveyOptions
            .Where(a => a.SurveyQuestionId == questionId)
            .ExecuteDeleteAsync(token);
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

}

