using SurveyBasket.Shared.Errors;

namespace SurveyBasket.Services.SurveyQuestions;

public class SurveyQuestionService(
    ISurveyQuestionRepository questionsRepo,
    ISurveyRepository surveyRepo,
    IUserSubmissionsRepository submissionRepo,
    ILogger<SurveyQuestionService> logger) : ISurveyQuestionService
{
    public async Task<Result<SurveyQuestionResponse>> AddAsync(int surveyId, CreateSurveyQuestionRequest request, CancellationToken token = default)
    {
        logger.LogInformation("Adding question to survey ID {SurveyId}", surveyId);

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
        {
            logger.LogWarning("Survey With {surveyId} Not Found", surveyId);
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.SurveyNotFound());

        }
        if (await questionsRepo.IsDuplicateSurveyQuestionAsync(request.Content, token))
        {
            logger.LogWarning("There are Already a Question with same Content as this Question({Content})", request.Content);
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.ConflictSurveyQuestion());
        }

        var question = request.Adapt<SurveyQuestion>();
        question.SurveyId = surveyId;

        var created = await questionsRepo.AddAsync(question, token);
        if (created is null)
        {
            logger.LogError("Failed to create survey question for survey ID {SurveyId}", surveyId);
            return Result.Failure<SurveyQuestionResponse>(SystemError.Database());
        }

        var fullQuestion = await questionsRepo.GetQuestionWithOptionsAsync(surveyId, created.Id, token);
        logger.LogInformation("Successfully added question ID {QuestionId} to survey ID {SurveyId}", created.Id, surveyId);

        return Result.Success(fullQuestion.Adapt<SurveyQuestionResponse>());
    }

    public async Task<Result<ICollection<SurveyQuestionResponse>>> GetAllAsync(int surveyId, CancellationToken token = default)
    {
        logger.LogInformation("Getting all questions for survey ID {SurveyId}", surveyId);

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyQuestionError.SurveyNotFound());

        var questions = await questionsRepo.GetAllAsync(surveyId, token);
        if (questions is null)
        {
            logger.LogError("Database error while fetching questions for survey ID {SurveyId}", surveyId);
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SystemError.Database());
        }

        logger.LogInformation("Found {Count} questions for survey ID {SurveyId}", questions.Count, surveyId);
        return Result.Success(questions.Adapt<ICollection<SurveyQuestionResponse>>());
    }

    public async Task<Result<SurveyQuestionResponse>> GetByIdAsync(int surveyId, int questionId, CancellationToken token = default)
    {
        logger.LogInformation("Fetching question ID {QuestionId} for survey ID {SurveyId}", questionId, surveyId);

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.SurveyNotFound());

        if (!await questionsRepo.ExistByIdAsync(surveyId, questionId, token))
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.SurveyQuestionNotFound());

        var question = await questionsRepo.GetQuestionWithOptionsAsync(surveyId, questionId, token);
        if (question is null)
            return Result.Failure<SurveyQuestionResponse>(SystemError.Database());

        return Result.Success(question.Adapt<SurveyQuestionResponse>());
    }

    public async Task<Result> RestoreSurveyQuestion(int surveyId, int questionId, CancellationToken token = default)
    {
        logger.LogInformation("Restoring deleted question ID {QuestionId} in survey ID {SurveyId}", questionId, surveyId);

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
            return Result.Failure(SurveyQuestionError.SurveyNotFound());

        if (!await questionsRepo.ExistByIdAsyncIgnoredFilter(surveyId, questionId, token))
            return Result.Failure(SurveyQuestionError.SurveyQuestionNotFound());

        var question = await questionsRepo.GetByIdIgnoringDeletionFilterAsync(surveyId, questionId, token);
        if (question is null)
            return Result.Failure(SystemError.Database());

        question.IsDeleted = false;
        question.DeletedBy = null;
        question.DeletedById = null;
        question.DeletedOn = null;

        bool restored = await questionsRepo.UpdateEntityAsync(question, token);
        if (!restored)
            return Result.Failure(SystemError.Database());

        logger.LogInformation("Successfully restored question ID {QuestionId} in survey ID {SurveyId}", questionId, surveyId);
        return Result.Success();
    }

    public async Task<Result> DeleteSurveyQuestionAsync(int surveyId, int questionId, CancellationToken token = default)
    {
        logger.LogInformation("Deleting question ID {QuestionId} in survey ID {SurveyId}", questionId, surveyId);

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
            return Result.Failure(SurveyQuestionError.SurveyNotFound());

        var question = await questionsRepo.GetByIdAsync(surveyId, questionId, token);
        if (question is null)
            return Result.Failure(SurveyQuestionError.SurveyQuestionNotFound());

        bool deleted = await questionsRepo.DeleteAsync(question, token);
        if (!deleted)
            return Result.Failure(SystemError.Database());

        logger.LogInformation("Deleted question ID {QuestionId} in survey ID {SurveyId}", questionId, surveyId);
        return Result.Success();
    }

    public async Task<Result> UpdateSurveyQuestionAsync(int surveyId, int questionId, UpdateSurveyQuestionRequest updateRequest, CancellationToken token = default)
    {
        logger.LogInformation("Updating question ID {QuestionId} for survey ID {SurveyId}", questionId, surveyId);

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
            return Result.Failure(SurveyQuestionError.SurveyNotFound());

        if (!await questionsRepo.ExistByIdAsync(surveyId, questionId, token))
            return Result.Failure(SurveyQuestionError.SurveyQuestionNotFound());

        if (await questionsRepo.ExistByContentWithDifferentId(surveyId, questionId, updateRequest.Content, token))
            return Result.Failure(SurveyQuestionError.ConflictSurveyQuestion());

        var question = updateRequest.Adapt<SurveyQuestion>();
        bool updated = await questionsRepo.UpdateWithOptionsResetAsync(surveyId, questionId, question, token);

        if (!updated)
            return Result.Failure(SystemError.Database());

        logger.LogInformation("Question ID {QuestionId} in survey ID {SurveyId} updated successfully", questionId, surveyId);
        return Result.Success();
    }

    public async Task<Result<ICollection<SurveyQuestionResponse>>> GetAvailableQuestionAsync(int surveyId, string userId, CancellationToken token)
    {
        logger.LogInformation("Fetching available questions for user {UserId} in survey ID {SurveyId}", userId, surveyId);

        if (await submissionRepo.IsSubmittedBeforeAsync(surveyId, userId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(UserSubmissionError.DuplicateSubmission());

        if (!await surveyRepo.ExistByIdAsync(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotFound());

        if (await surveyRepo.IsSurveyNotStarted(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("The survey has not started yet."));

        if (await surveyRepo.IsSurveyClosed(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.AlreadyClosed());

        if (!await surveyRepo.IsSurveyAvailable(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("Survey is not available or published."));

        var questions = await questionsRepo.GetAvailableQuestionAsync(surveyId, token);
        logger.LogInformation("Returning {Count} available questions for survey ID {SurveyId}", questions.Count, surveyId);

        return Result.Success(questions.Adapt<ICollection<SurveyQuestionResponse>>());
    }
}
