namespace SurveyBasket.Repositories;

public interface IUserSubmissionsRepository : IScopedService
{
    public Task<bool> SubmittedBeforeAsync(string userId, CancellationToken token = default);

}

public class UserSubmissionsRepository(AppDbContext db) : IUserSubmissionsRepository
{
    public async Task<bool> SubmittedBeforeAsync(string userId, CancellationToken token = default)
        => await db.UserSubmissions.AnyAsync(sub => sub.UserId == userId, token);



}

