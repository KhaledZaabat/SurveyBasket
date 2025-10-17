using Microsoft.AspNetCore.Identity;

namespace SurveyBasket.Services.UsersService;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<Result<UserProfileResponse>> GetUserProfile(string userId, CancellationToken cancellationToken = default)
    {
        UserProfileResponse result = await userManager.Users
      .Where(u => u.Id == userId)
      .ProjectToType<UserProfileResponse>()
      .FirstAsync(cancellationToken);
        return Result.Success<UserProfileResponse>(result);

    }
    public async Task PatchUserProfile(string userId, JsonPatchDocument<UpdateUserProfileRequest> patchDoc, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken);





        UpdateUserProfileRequest dto = user.Adapt<UpdateUserProfileRequest>();


        patchDoc.ApplyTo(dto);


        dto.Adapt(user);


        var result = await userManager.UpdateAsync(user);

        var response = user.Adapt<UserProfileResponse>();




    }


}

