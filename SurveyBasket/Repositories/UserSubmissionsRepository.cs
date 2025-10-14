

namespace SurveyBasket.Repositories;

public class UserSubmissionsRepository(AppDbContext db) : IUserSubmissionsRepository
{
    public async Task<bool> SubmittedBeforeAsync(int surveyId, string userId, CancellationToken token = default)
        => await db.UserSubmissions.AnyAsync(sub => sub.UserId == userId && sub.SurveyId == surveyId, token);

    public async Task<bool> AddAsync(UserSubmission submission, CancellationToken token = default)
    {
        await db.UserSubmissions.AddAsync(submission, token);
        await db.SaveChangesAsync(token);
        return true;
    }
    public async Task<List<(DateOnly submittedOn, int count)>> GetSubmissionsPerDayCountAsync(int surveyId, CancellationToken cancellationToken = default)
    {
        var results = await db.UserSubmissions
          .AsNoTracking()
          .Where(s => s.SurveyId == surveyId && !s.IsDeleted)
          .GroupBy(s => DateOnly.FromDateTime(s.SubmittedOn))
          .Select(g => new
          {
              SubmittedOn = g.Key,
              Count = g.Count()
          })
          .OrderBy(x => x.SubmittedOn)
          .ToListAsync(cancellationToken);

        return results.Select(x => (x.SubmittedOn, x.Count)).ToList();



    }

    public async Task<List<UserSubmission>?> GetSurveySubmissionsAsync(int surveyId, CancellationToken cancellationToken = default)
    {

        return await db.UserSubmissions.AsNoTracking().Where(s => s.SurveyId == surveyId)
              .Include(s => s.Survey)
              .Include(s => s.User)
              .Include(s => s.SubmissionDetails)
              .ThenInclude(sub => sub.Option)


              .ToListAsync(cancellationToken);

    }

    public async Task<List<SurveyStatistics>> GetSurveyStatistics(int surveyId, CancellationToken cancellationToken = default)
    {
        Dictionary<int, int> countDictionary = db.SubmissionDetails.Include(s => s.Question)
            .Where(d => d.Question.SurveyId == surveyId)
            .GroupBy(s => s.OptionId).Select(g => new { optionId = g.Key, count = g.Count() }).ToDictionary(u => u.optionId, u => u.count);

        var questions = await db.SurveyQuestions
       .AsNoTracking()
       .Where(q => q.SurveyId == surveyId)
       .Include(q => q.SurveyOptions)
       .OrderBy(q => q.Id)
       .ToListAsync(cancellationToken)
       ;
        return questions.Select(q => new SurveyStatistics(
         Question: q.Content,
         SelectedAnswers: q.SurveyOptions.Select(o => new AnswerStatistics(
             Answer: o.Content,
             Count: countDictionary.GetValueOrDefault(o.Id, 0)
         ))
     )).ToList();

    }

}




