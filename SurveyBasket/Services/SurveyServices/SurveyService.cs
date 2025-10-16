namespace SurveyBasket.Services.SurveyServices;

public class SurveyService(ISurveyRepository surveyRepository, ILogger<SurveyService> logger) : ISurveyService
{
    public async Task<Result<ICollection<SurveyResponse>>> GetAllAsync(CancellationToken token = default)
    {
        logger.LogInformation("Fetching all active surveys");
        var surveys = await surveyRepository.GetAllAsync(token);
        return Result.Success(surveys.Adapt<ICollection<SurveyResponse>>());
    }

    public async Task<Result<ICollection<SurveyResponse>>> GetAllIncludingDeletedAsync(CancellationToken token = default)
    {
        logger.LogInformation("Fetching all surveys including deleted ones");
        var surveys = await surveyRepository.GetAllIncludingDeletedAsync(token);
        return Result.Success(surveys.Adapt<ICollection<SurveyResponse>>());
    }

    public async Task<Result<SurveyResponse>> GetByIdAsync(int id, CancellationToken token = default)
    {
        logger.LogInformation("Fetching survey by ID {SurveyId}", id);
        var survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
        {
            logger.LogWarning("Survey with ID {SurveyId} not found", id);
            return Result.Failure<SurveyResponse>(SurveyError.NotFound());
        }

        return Result.Success(survey.Adapt<SurveyResponse>());
    }

    public async Task<Result<SurveyResponse>> AddAsync(CreateSurveyRequest request, CancellationToken token = default)
    {
        logger.LogInformation("Adding survey titled '{Title}'", request.Title);

        if (await surveyRepository.ExistByTitleAsync(request.Title, token))
        {
            logger.LogWarning("Survey titled '{Title}' already exists", request.Title);
            return Result.Failure<SurveyResponse>(SurveyError.Conflict());
        }

        var survey = request.Adapt<Survey>();
        var addedSurvey = await surveyRepository.AddAsync(survey, token);

        logger.LogInformation("Survey '{Title}' created successfully with ID {SurveyId}", survey.Title, addedSurvey.Id);
        return Result.Success(addedSurvey.Adapt<SurveyResponse>());
    }

    public async Task<Result> UpdateAsync(int id, UpdateSurveyRequest request, CancellationToken token = default)
    {
        logger.LogInformation("Updating survey ID {SurveyId}", id);
        Survey? survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
        {
            logger.LogWarning("Survey ID {SurveyId} not found for update", id);
            return Result.Failure(SurveyError.NotFound());
        }

        if (await surveyRepository.ExistByTitleWithDifferentIdAsync(request.Title, id, token))
        {
            logger.LogWarning("Another survey already uses the title '{Title}'", request.Title);
            return Result.Failure(SurveyError.Conflict());
        }

        survey.Title = request.Title;
        survey.Summary = request.Summary;
        survey.StartsAt = request.StartsAt;
        survey.EndsAt = request.EndsAt;

        await surveyRepository.UpdateAsync(survey, token);
        logger.LogInformation("Survey ID {SurveyId} updated successfully", id);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken token = default)
    {
        logger.LogInformation("Deleting survey ID {SurveyId}", id);
        var survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
        {
            logger.LogWarning("Survey ID {SurveyId} not found for deletion", id);
            return Result.Failure(SurveyError.NotFound());
        }

        await surveyRepository.DeleteAsync(survey, token);
        logger.LogInformation("Survey ID {SurveyId} deleted successfully", id);
        return Result.Success();
    }

    public async Task<Result> TogglePublishAsync(int id, CancellationToken token = default)
    {
        logger.LogInformation("Toggling publish state for survey ID {SurveyId}", id);
        var survey = await surveyRepository.GetByIdAsync(id, token);
        if (survey is null)
        {
            logger.LogWarning("Survey ID {SurveyId} not found for publish toggle", id);
            return Result.Failure(SurveyError.NotFound());
        }

        survey.Status = new PublishStatus(!survey.Status.IsPublished);
        await surveyRepository.UpdateAsync(survey, token);
        logger.LogInformation("Survey ID {SurveyId} publish status changed to {Status}", id, survey.Status.IsPublished);
        return Result.Success();
    }

    public async Task<Result<ICollection<SurveyResponse>>> GetCurrentSurveysAsync(CancellationToken token = default)
    {
        logger.LogInformation("Fetching currently active surveys");
        var surveys = await surveyRepository.GetCurrentSurveysAsync(token);
        if (surveys == null)
        {
            logger.LogError("Database returned null for current surveys");
            return Result.Failure<ICollection<SurveyResponse>>(SystemError.Database());
        }

        return Result.Success(surveys.Adapt<ICollection<SurveyResponse>>());
    }

    public async Task<Result> RestoreSurveyAsync(int surveyId, CancellationToken token = default)
    {
        logger.LogInformation("Restoring deleted survey ID {SurveyId}", surveyId);
        Survey? survey = await surveyRepository.GetByIdAsyncIncludingDeletedAsync(surveyId, token);
        if (survey is null)
        {
            logger.LogWarning("Survey ID {SurveyId} not found for restore", surveyId);
            return Result.Failure(SurveyError.NotFound());
        }

        survey.IsDeleted = false;
        survey.DeletedBy = null;
        survey.DeletedOn = null;
        survey.DeletedById = null;
        await surveyRepository.UpdateAsync(survey, token);

        logger.LogInformation("Survey ID {SurveyId} restored successfully", surveyId);
        return Result.Success();
    }
}
