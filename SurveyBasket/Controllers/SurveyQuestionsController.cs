using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Helpers;
using SurveyBasket.Services.SurveyQuestions;

namespace SurveyBasket.Controllers;

[Route("api/surveys/{surveyId}/[controller]")]
[ApiController]
[Authorize]
public class SurveyQuestionsController(ISurveyQuestionService questionService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<ICollection<SurveyQuestionResponse>>> GetAllAsync(int surveyId
        , CancellationToken token = default)
        => (await questionService.GetAllAsync(surveyId, token)).ToActionResult(HttpContext);



    [HttpGet("{questionId}")]
    public async Task<ActionResult<SurveyQuestionResponse>> GetByIdAsync(
        [FromRoute] int surveyId,
        [FromRoute] int questionId,
        CancellationToken token = default)
        => (await questionService.GetByIdAsync(surveyId, questionId, token)).ToActionResult(HttpContext);

    [HttpPost]
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
    public async Task<IActionResult> RestoreSurveyQuestion([FromRoute] int surveyId,
        [FromRoute] int questionId,
        CancellationToken token = default)
    {
        Result result = await questionService.RestoreSurveyQuestion(surveyId, questionId, token);
        if (result is SuccessResult) return NoContent();
        return result.ToProblem(HttpContext);

    }
    [HttpDelete("{questionId}")]
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
    public async Task<IActionResult> UpdateSurveyQuestion([FromRoute] int surveyId,
        [FromRoute] int questionId, UpdateSurveyQuestionRequest updateRequest,
        CancellationToken token = default)
    {
        Result result = await questionService.UpdateSurveyQuestionAsync(surveyId, questionId, updateRequest, token);
        if (result is SuccessResult) return NoContent();
        return result.ToProblem(HttpContext);
    }
}



