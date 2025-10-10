using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Contracts.Polls.Responses;
using SurveyBasket.Helpers;

namespace SurveyBasket.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService _pollService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PollResponse>>> GetAll(CancellationToken token = default)
        => (await _pollService.GetAll(token)).ToActionResult(this);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PollResponse>> GetById([FromRoute] int id, CancellationToken token = default)
        => (await _pollService.GetById(id, token)).ToActionResult(this);

    [HttpPost]
    public async Task<ActionResult<PollResponse>> Add([FromBody] CreatePollRequest request, CancellationToken token = default)
    {
        var result = await _pollService.Add(request, token);
        if (result is SuccessResult<PollResponse> success)
            return CreatedAtAction(nameof(GetById), new { id = success.Value.Id }, success.Value);
        return result.ToActionResult(this);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePollRequest request, CancellationToken token = default)
    {
        var result = await _pollService.Update(id, request, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token = default)
    {
        var result = await _pollService.Delete(id, token);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(this);
    }

    [HttpPatch("{id:int}/toggle-publish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _pollService.TogglePublish(id, cancellationToken);
        if (result is SuccessResult)
            return NoContent();
        return result.ToActionResult(this);
    }
}
