namespace SurveyBasket.Repositories;

public class UserSubmissionsRepository(AppDbContext db) : IUserSubmissionsRepository
{
    public async Task<bool> SubmittedBeforeAsync(int surveyId,string userId, CancellationToken token = default)
        => await db.UserSubmissions.AnyAsync(sub => sub.UserId == userId&& sub.SurveyId==surveyId, token);



}

