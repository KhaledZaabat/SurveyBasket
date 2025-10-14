

namespace SurveyBasket.Services.Results;

public class ResultService(IUserSubmissionsRepository userSubmissionsRepository, ISurveyRepository surveyRepository) : IResultService
{


    public async Task<Result<SurveySubmissionsResponse>> GetSurveySubmissionsAsync(int surveyId, CancellationToken cancellationToken = default)
    {
        Survey? survey = await surveyRepository.GetByIdAsync(surveyId, cancellationToken);
        if (survey is null)
            return Result.Failure<SurveySubmissionsResponse>(SurveyError.NotFound());

        List<UserSubmission>? submissions = await userSubmissionsRepository.GetSurveySubmissionsAsync(surveyId, cancellationToken);

        if (submissions is null || submissions.Count == 0)
            return Result.Success(new SurveySubmissionsResponse(
                Title: survey.Title,
                Submissions: []));
        return Result.Success<SurveySubmissionsResponse>(submissions.Adapt<SurveySubmissionsResponse>());



    }

    public async Task<Result<List<SubmissionsPerDayResponse>>> GetSubmissionsPerDayCountAsync(int surveyId, CancellationToken cancellationToken = default)
    {

        bool survey = await surveyRepository.ExistByIdAsync(surveyId, cancellationToken);
        if (!survey)
            return Result.Failure<List<SubmissionsPerDayResponse>>(SurveyError.NotFound());
        List<(DateOnly submittedOn, int count)> values = await userSubmissionsRepository.GetSubmissionsPerDayCountAsync(surveyId, cancellationToken);
        var response = values
               .Select(t => new SubmissionsPerDayResponse(
                   Date: t.submittedOn,
                   NumberOfSubmissions: t.count))
               .ToList();

        return Result.Success(response);


    }

    public async Task<Result<List<SurveyStatistics>>> GetSurveyStatistics(int surveyId, CancellationToken cancellationToken = default)
    {
        Survey? survey = await surveyRepository.GetByIdAsync(surveyId, cancellationToken);
        if (survey is null)
            return Result.Failure<List<SurveyStatistics>>(SurveyError.NotFound());

        List<SurveyStatistics> res = await userSubmissionsRepository.GetSurveyStatistics(surveyId, cancellationToken);
        if (res is null || res.Count == 0)
            return Result.Success<List<SurveyStatistics>>([]
               );
        return Result.Success<List<SurveyStatistics>>(res);

    }


}

