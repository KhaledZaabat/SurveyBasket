using Microsoft.AspNetCore.Identity;
using SurveyBasket.Shared.Errors;

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

    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);

        var result = await userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var errors = result.Errors;
        if (result.Errors.Any(e => e.Code.Contains("PasswordMismatch")))
            return Result.Failure(UserError.InvalidCredentials("Current password is incorrect"));

        if (result.Errors.Any(e => e.Code.Contains("PasswordTooShort") || e.Code.Contains("PasswordRequires")))
            return Result.Failure(UserError.InvalidSubmission("New password does not meet requirements"));


        return Result.Failure(UserError.InvalidSubmission("Failed to change password"));


    }
}

