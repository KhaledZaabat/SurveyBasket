
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

        [HttpPatch("profile")]
        public async Task<IActionResult> PatchUserProfile(
            [FromBody] JsonPatchDocument<UpdateUserProfileRequest> patchDoc,
            CancellationToken cancellationToken)
        {
            if (patchDoc is null)
                return BadRequest("Invalid patch document.");

            var userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            await userService.PatchUserProfile(userId, patchDoc, cancellationToken);

            return NoContent();
        }

    }
}
