using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Helpers;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SurveysController(ISurveyService _surveyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ICollection<SurveyResponse>>> GetAll(CancellationToken token = default)
        => (await _surveyService.GetAllAsync(token)).ToActionResult(context: HttpContext);

    [HttpGet("admin/all")]
    // [Authorize(Roles = "Admin")] later
    public async Task<ActionResult<ICollection<SurveyResponse>>> GetAllIncludingDeleted(CancellationToken token = default)
         => (await _surveyService.GetAllIncludingDeletedAsync(token))
             .ToActionResult(context: HttpContext);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SurveyResponse>> GetById([FromRoute] int id, CancellationToken token = default)
        => (await _surveyService.GetByIdAsync(id, token)).ToActionResult(context: HttpContext);

    [HttpPost]
    public async Task<ActionResult<SurveyResponse>> Add([FromBody] CreateSurveyRequest request, CancellationToken token = default)
    {
        Result<SurveyResponse> result = await _surveyService.AddAsync(request, token);
        if (result is SuccessResult<SurveyResponse> success)
            return CreatedAtAction(nameof(GetById), new { id = success.Value.Id }, success.Value);



        return result.ToActionResult(context: HttpContext);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateSurveyRequest request, CancellationToken token = default)
    {
        Result result = await _surveyService.UpdateAsync(id, request, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(context: HttpContext);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token = default)
    {
        Result result = await _surveyService.DeleteAsync(id, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(context: HttpContext);
    }

    [HttpPatch("{id:int}/toggle-publish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        Result result = await _surveyService.TogglePublishAsync(id, cancellationToken);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(context: HttpContext);
    }




}
