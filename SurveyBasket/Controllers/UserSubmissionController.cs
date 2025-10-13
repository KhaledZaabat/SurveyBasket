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

        [HttpGet("/api/Surveys/{surveyId}/Available")]

        public async Task<ActionResult<ICollection<SurveyQuestionResponse>>> GetAvailableQuestionAsync([FromRoute] int SurveyId, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            Result<ICollection<SurveyQuestionResponse>> result = await questionService.GetAvailableQuestionAsync(SurveyId, userId!, cancellationToken);

            return result.ToActionResult<ICollection<SurveyQuestionResponse>>(HttpContext);

        }


    }
}
