using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService _pollService) : ControllerBase
    {






        [HttpGet]
        public async Task<ActionResult<ICollection<PollResponse>>> Get(CancellationToken token = default)
        {
            var polls = await _pollService.GetAll(token);
            return Ok(polls);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PollResponse>> GetById([FromRoute] int id, CancellationToken token = default)
        {
            var poll = await _pollService.GetById(id, token);
            return poll is null ? NotFound() : Ok(poll);
        }

        [HttpPost]
        public async Task<ActionResult<PollResponse>> Add([FromBody] CreatePollRequest request, CancellationToken token = default)
        {
            var poll = await _pollService.Add(request, token);
            return CreatedAtAction(nameof(GetById), new { id = poll.Id }, poll);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdatePollRequest request, CancellationToken token = default)
        {
            var updated = await _pollService.Update(id, request, token);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken token = default)
        {
            var deleted = await _pollService.Delete(id, token);
            return deleted ? NoContent() : NotFound();
        }
        [HttpPatch("{id:int}/toggle-publish")]
        public async Task<IActionResult> TogglePublish(
            [FromRoute] int id,
            CancellationToken cancellationToken = default)
        {
            var toggled = await _pollService.TogglePublish(id, cancellationToken);
            return toggled ? NoContent() : NotFound();
        }
    }
}

