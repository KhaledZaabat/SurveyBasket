
namespace SurveyBasket.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {




        // GET: api/users/profile
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileResponse>> GetUserProfile(CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var result = await userService.GetUserProfile(userId, cancellationToken);

            return result.ToActionResult<UserProfileResponse>(HttpContext);
        }

        // POST: api/user/change-password
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            return (await userService.ChangePasswordAsync(userId, request)).ToActionResult();

        }

    }
}
