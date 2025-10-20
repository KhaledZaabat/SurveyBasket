using SurveyBasket.Consts;
using SurveyBasket.Services.SurveyQuestions;
using SurveyBasket.Services.UserSubmissionServices;

namespace SurveyBasket.Controllers
{
    [Route("api/Surveys/{surveyId}/Submissions")]
    [ApiController]
    [Authorize(Roles = DefaultRoles.Member)]
    public class UserSubmissionController(ISurveyQuestionService questionService, IUserSubmissionService submissionService) : ControllerBase
    {

        /// <summary>
        /// Gets all The questions for a survey (must be open and published) with its options
        /// </summary>

        [HttpGet("/api/Surveys/{surveyId}/Available")]

        public async Task<ActionResult<ICollection<SurveyQuestionResponse>>> GetAvailableQuestionAsync([FromRoute] int SurveyId, CancellationToken cancellationToken)
        {
            string? userId = User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Result<ICollection<SurveyQuestionResponse>> result = await questionService.GetAvailableQuestionAsync(SurveyId, userId!, cancellationToken);

            return result.ToActionResult<ICollection<SurveyQuestionResponse>>(HttpContext);

        }

        /// <summary>
        /// Adds a new submission for a survey by the currently authenticated user.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddSubmissionAsync(
            [FromRoute] int surveyId,
            [FromBody] UserSubmissionRequest request,
            CancellationToken cancellationToken)
        {
            string? userId = User.GetUserId();

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Result result = await submissionService.AddAsync(surveyId, userId, request, cancellationToken);
            if (result is SuccessResult) return NoContent();
            return result.ToProblem(HttpContext);
        }


    }
}
