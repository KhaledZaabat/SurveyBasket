using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Helpers;
using SurveyBasket.Services.SurveyQuestions;
using System.Security.Claims;

namespace SurveyBasket.Controllers
{
    [Route("api/Surveys/{surveyId}/Submissions")]
    [ApiController]
    [Authorize]
    public class UserSubmissionController(ISurveyQuestionService questionService) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Result<ICollection<SurveyQuestionResponse>> result = await questionService.GetAvailableQuestionAsync(pollId, userId!, cancellationToken);

            return result.ToProblem(HttpContext);

        }


    }
}
