
namespace SurveyBasket.Controllers
{
    [Route("api/Surveys/{surveyId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        /// <summary>
        /// Gets all submissions for a specific survey, including usernames and selected options.
        /// </summary>
        [HttpGet("row-data")]
        public async Task<ActionResult<SurveySubmissionsResponse>> SurveySubmissions(
            [FromRoute] int surveyId,
            CancellationToken cancellationToken)
        {
            var result = await _resultService.GetSurveySubmissionsAsync(surveyId, cancellationToken);
            return result.ToActionResult<SurveySubmissionsResponse>(HttpContext);
        }

        /// <summary>
        /// Gets the number of submissions per day for a specific survey.
        /// </summary>
        [HttpGet("submissions-per-day")]
        public async Task<ActionResult<List<SubmissionsPerDayResponse>>> GetSubmissionsPerDayCount(
            [FromRoute] int surveyId,
            CancellationToken cancellationToken)
        {
            Result<List<SubmissionsPerDayResponse>> result =
                await _resultService.GetSubmissionsPerDayCountAsync(surveyId, cancellationToken);

            return result.ToActionResult<List<SubmissionsPerDayResponse>>(HttpContext);
        }

        /// <summary>
        /// Gets statistics of each survey question — how many times each option was selected.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<List<SurveyStatistics>>> GetSurveyStatistics(
            [FromRoute] int surveyId,
            CancellationToken cancellationToken)
        {
            Result<List<SurveyStatistics>> result =
                await _resultService.GetSurveyStatistics(surveyId, cancellationToken);

            return result.ToActionResult<List<SurveyStatistics>>(HttpContext);
        }
    }
}
