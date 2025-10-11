using SurveyBasket.Contracts.Question.Requests;
using SurveyBasket.Helpers;
using SurveyBasket.Services.Questions;

namespace SurveyBasket.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<ICollection<QuestionResponse>>> GetAllAsync(int pollId
            , CancellationToken token = default)
            => (await questionService.GetAllAsync(pollId, token)).ToActionResult();



        [HttpGet("{questionId}")]
        public async Task<ActionResult<QuestionResponse>> GetByIdAsync(
            [FromRoute] int pollId,
            [FromRoute] int questionId,
            CancellationToken token = default)
            => (await questionService.GetByIdAsync(pollId, questionId, token)).ToActionResult();

        [HttpPost]
        public async Task<ActionResult<QuestionResponse>> AddQuestionAsync([FromRoute] int pollId,
            [FromBody] CreateQuestionRequest request,
             CancellationToken token = default)
        {

            Result<QuestionResponse> result = await questionService.AddAsync(pollId, request, token);
            if (result is SuccessResult<QuestionResponse> valuedResult)
                return CreatedAtAction(nameof(GetByIdAsync), routeValues: new { pollId, questionId = valuedResult.Value.Id }, valuedResult.Value);

            return result.ToProblem();


        }
    }
}
