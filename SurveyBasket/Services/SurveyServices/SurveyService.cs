using Mapster;


namespace SurveyBasket.Services.SurveyServices;

public class SurveyService(ISurveyRepository surveyRepository) : ISurveyService
{
    public async Task<Result<ICollection<SurveyResponse>>> GetAllAsync(CancellationToken token = default)
    {
        var surveys = await surveyRepository.GetAllAsync(token);
        return Result.Success(surveys.Adapt<ICollection<SurveyResponse>>());
    }

    public async Task<Result<ICollection<SurveyResponse>>> GetAllIncludingDeletedAsync(CancellationToken token = default)
    {
        var surveys = await surveyRepository.GetAllIncludingDeletedAsync(token);
        return Result.Success(surveys.Adapt<ICollection<SurveyResponse>>());
    }
    public async Task<Result<SurveyResponse>> GetByIdAsync(int id, CancellationToken token = default)
    {
        var survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
            return Result.Failure<SurveyResponse>(SurveyError.NotFound());

        return Result.Success(survey.Adapt<SurveyResponse>());
    }

    public async Task<Result<SurveyResponse>> AddAsync(CreateSurveyRequest request, CancellationToken token = default)
    {


        if (await surveyRepository.ExistByTitleAsync(request.Title, token))
            return Result.Failure<SurveyResponse>(SurveyError.Conflict());

        var survey = request.Adapt<Survey>();
        var addedSurvey = await surveyRepository.AddAsync(survey, token);

        return Result.Success(addedSurvey.Adapt<SurveyResponse>());
    }

    public async Task<Result> UpdateAsync(int id, UpdateSurveyRequest request, CancellationToken token = default)
    {
        Survey? survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
            return Result.Failure(SurveyError.NotFound());



        if (await surveyRepository.ExistByTitleWithDifferentIdAsync(request.Title, id, token))
            return Result.Failure(SurveyError.Conflict());

        // Apply updates
        survey.Title = request.Title;
        survey.Summary = request.Summary;
        survey.StartsAt = request.StartsAt;
        survey.EndsAt = request.EndsAt;

        await surveyRepository.UpdateAsync(survey, token);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken token = default)
    {
        var survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
            return Result.Failure(SurveyError.NotFound());

        await surveyRepository.DeleteAsync(survey, token);
        return Result.Success();
    }

    public async Task<Result> TogglePublishAsync(int id, CancellationToken token = default)
    {
        var survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
            return Result.Failure(SurveyError.NotFound());

        survey.Status = new PublishStatus(!survey.Status.IsPublished);
        await surveyRepository.UpdateAsync(survey, token);

        return Result.Success();
    }
}
