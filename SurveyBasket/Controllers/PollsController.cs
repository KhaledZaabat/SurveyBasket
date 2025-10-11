using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Contracts.Polls.Responses;
using SurveyBasket.Contracts.Question.Requests;
using SurveyBasket.Helpers;
using SurveyBasket.Services.Questions;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService _pollService, IQuestionService _questionService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PollResponse>>> GetAll(CancellationToken token = default)
        => (await _pollService.GetAllAsync(token)).ToActionResult(context: HttpContext);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PollResponse>> GetById([FromRoute] int id, CancellationToken token = default)
        => (await _pollService.GetByIdAsync(id, token)).ToActionResult(context: HttpContext);

    [HttpPost]
    public async Task<ActionResult<PollResponse>> Add([FromBody] CreatePollRequest request, CancellationToken token = default)
    {
        Result<PollResponse> result = await _pollService.AddAsync(request, token);
        if (result is SuccessResult<PollResponse> success)
            return CreatedAtAction(nameof(GetById), new { id = success.Value.Id }, success.Value);



        return result.ToActionResult(context: HttpContext);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePollRequest request, CancellationToken token = default)
    {
        Result result = await _pollService.UpdateAsync(id, request, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(context: HttpContext);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token = default)
    {
        Result result = await _pollService.DeleteAsync(id, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(context: HttpContext);
    }

    [HttpPatch("{id:int}/toggle-publish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        Result result = await _pollService.TogglePublishAsync(id, cancellationToken);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(context: HttpContext);
    }


    [HttpPost("{id}/Add")]


    public async Task<ActionResult<QuestionResponse>> AddQuestionAsync([FromRoute] int id, [FromBody] CreateQuestionRequest request)
    {

        Result<QuestionResponse> result = await _questionService.AddAsync(id, request);
        if (result is SuccessResult<QuestionResponse> valuedResult)
            return Ok(valuedResult.Value);

        return result.ToProblem();


    }
}
