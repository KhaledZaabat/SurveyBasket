namespace SurveyBasket.Services.UsersService;

public interface IUserService : IScopedService
{
    public Task<Result<UserProfileResponse>> GetUserProfile(string userId, CancellationToken cancellationToken = default);
    public Task PatchUserProfile(string userId, JsonPatchDocument<UpdateUserProfileRequest> patchDoc, CancellationToken cancellationToken = default);

}

