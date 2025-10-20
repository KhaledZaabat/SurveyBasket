using SurveyBasket.Auhtentication_Providers.Filters;
using SurveyBasket.Consts;
using SurveyBasket.Services.SurveyQuestions;

namespace SurveyBasket.Controllers;

[Route("api/surveys/{surveyId}/Questions")]
[ApiController]
[Authorize]
public class SurveyQuestionsController(ISurveyQuestionService questionService) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Questions.Read)]
    public async Task<ActionResult<ICollection<SurveyQuestionResponse>>> GetAllAsync(int surveyId
        , CancellationToken token = default)
  => (await questionService.GetAllAsync(surveyId, token)).ToActionResult(HttpContext);

    [HttpGet("{questionId}")]
    [HasPermission(Permissions.Questions.Read)]
    public async Task<ActionResult<SurveyQuestionResponse>> GetByIdAsync(
        [FromRoute] int surveyId,
        [FromRoute] int questionId,
        CancellationToken token = default)
        => (await questionService.GetByIdAsync(surveyId, questionId, token)).ToActionResult(HttpContext);

    [HttpPost]
    [HasPermission(Permissions.Questions.Add)]
    public async Task<ActionResult<SurveyQuestionResponse>> AddSurveyQuestionAsync([FromRoute] int surveyId,
        [FromBody] CreateSurveyQuestionRequest request,
         CancellationToken token = default)
    {
        Result<SurveyQuestionResponse> result = await questionService.AddAsync(surveyId, request, token);
        if (result is SuccessResult<SurveyQuestionResponse> valuedResult)
            return CreatedAtAction(nameof(GetByIdAsync), routeValues: new { surveyId, questionId = valuedResult.Value.Id }, valuedResult.Value);
        return result.ToProblem(HttpContext);
    }

    [HttpPatch("{questionId}/restore")]
    [HasPermission(Permissions.Questions.Update)]
    public async Task<IActionResult> RestoreSurveyQuestion([FromRoute] int surveyId,
        [FromRoute] int questionId,
        CancellationToken token = default)
    {
        Result result = await questionService.RestoreSurveyQuestion(surveyId, questionId, token);
        if (result is SuccessResult) return NoContent();
        return result.ToProblem(HttpContext);
    }

    [HttpDelete("{questionId}/delete")]
    [HasPermission(Permissions.Questions.Update)]
    public async Task<IActionResult> DeleteSurveyQuestion([FromRoute] int surveyId,
        [FromRoute] int questionId,
        CancellationToken token = default)
    {
        var result = await questionService.DeleteSurveyQuestionAsync(surveyId, questionId, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToProblem(HttpContext);
    }

    [HttpPut("{questionId}")]
    [HasPermission(Permissions.Questions.Update)]
    public async Task<IActionResult> UpdateQuestion([FromRoute] int surveyId,
       [FromRoute] int questionId, UpdateSurveyQuestionRequest updateRequest,
       CancellationToken token = default)
    {
        Result result = await questionService.UpdateSurveyQuestionAsync(surveyId, questionId, updateRequest, token);
        if (result is SuccessResult) return NoContent();
        return result.ToProblem(HttpContext);
    }
}