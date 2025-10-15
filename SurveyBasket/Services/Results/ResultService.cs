namespace SurveyBasket.Services.Results
{
    public class ResultService(
        IUserSubmissionsRepository userSubmissionsRepository,
        ISurveyRepository surveyRepository,
        ILogger<ResultService> logger) : IResultService
    {
        public async Task<Result<SurveySubmissionsResponse>> GetSurveySubmissionsAsync(int surveyId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching submissions for survey ID {SurveyId}", surveyId);

            Survey? survey = await surveyRepository.GetByIdAsync(surveyId, cancellationToken);
            if (survey is null)
            {
                logger.LogWarning("Survey with ID {SurveyId} not found", surveyId);
                return Result.Failure<SurveySubmissionsResponse>(SurveyError.NotFound());
            }

            List<UserSubmission>? submissions = await userSubmissionsRepository.GetSurveySubmissionsAsync(surveyId, cancellationToken);

            if (submissions is null || submissions.Count == 0)
            {
                logger.LogInformation("Survey ID {SurveyId} has no submissions", surveyId);
                return Result.Success(new SurveySubmissionsResponse(
                    Title: survey.Title,
                    Submissions: []));
            }

            logger.LogInformation("Survey ID {SurveyId} retrieved with {SubmissionCount} submissions", surveyId, submissions.Count);
            return Result.Success<SurveySubmissionsResponse>(submissions.Adapt<SurveySubmissionsResponse>());
        }

        public async Task<Result<List<SubmissionsPerDayResponse>>> GetSubmissionsPerDayCountAsync(int surveyId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching submissions per day for survey ID {SurveyId}", surveyId);

            bool exists = await surveyRepository.ExistByIdAsync(surveyId, cancellationToken);
            if (!exists)
            {
                logger.LogWarning("Survey with ID {SurveyId} not found while getting per-day submission counts", surveyId);
                return Result.Failure<List<SubmissionsPerDayResponse>>(SurveyError.NotFound());
            }

            List<(DateOnly submittedOn, int count)> values = await userSubmissionsRepository.GetSubmissionsPerDayCountAsync(surveyId, cancellationToken);

            logger.LogInformation("Survey ID {SurveyId} has {DayCount} days with submissions", surveyId, values.Count);

            var response = values
                .Select(t => new SubmissionsPerDayResponse(
                    Date: t.submittedOn,
                    NumberOfSubmissions: t.count))
                .ToList();

            return Result.Success(response);
        }

        public async Task<Result<List<SurveyStatistics>>> GetSurveyStatistics(int surveyId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching survey statistics for survey ID {SurveyId}", surveyId);

            Survey? survey = await surveyRepository.GetByIdAsync(surveyId, cancellationToken);
            if (survey is null)
            {
                logger.LogWarning("Survey with ID {SurveyId} not found while fetching statistics", surveyId);
                return Result.Failure<List<SurveyStatistics>>(SurveyError.NotFound());
            }

            List<SurveyStatistics> stats = await userSubmissionsRepository.GetSurveyStatistics(surveyId, cancellationToken);

            if (stats is null || stats.Count == 0)
            {
                logger.LogInformation("Survey ID {SurveyId} has no available statistics", surveyId);
                return Result.Success<List<SurveyStatistics>>([]);
            }

            logger.LogInformation("Survey ID {SurveyId} retrieved with {StatsCount} statistics entries", surveyId, stats.Count);
            return Result.Success(stats);
        }
    }
}
