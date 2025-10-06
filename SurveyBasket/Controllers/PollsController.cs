namespace SurveyBasket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollsController(IPollService _pollService) : ControllerBase
    {




        [HttpGet]
        public ActionResult<List<Poll>> Get()
        {

            return Ok(_pollService.GetAll());
        }

        [HttpGet("{id:int}")]
        public ActionResult<Poll> GetbyId(int id)
        {

            Poll? poll = _pollService.GetById(id);
            return poll is null ? NotFound() : Ok(poll);
        }

        [HttpPost]
        public ActionResult<Poll> Add([FromBody] Poll poll)
        {

            Poll? ans = _pollService.Add(poll);
            if (ans is null)
            {
                return StatusCode(500, "Unable to create poll."); // Internal Server Error
            }
            return CreatedAtAction(nameof(GetbyId), new { id = ans.Id }, ans);
        }
        [HttpPut("{id:int}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Poll poll)
        {
            bool Updated = _pollService.Update(id, poll);
            if (!Updated) return NotFound();
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            bool Updated = _pollService.Delete(id);
            if (!Updated) return NotFound();
            return NoContent();

        }

    }
}
