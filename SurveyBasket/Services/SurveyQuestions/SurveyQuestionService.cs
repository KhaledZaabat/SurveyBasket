using Mapster;
using SurveyBasket.Shared.Errors;

namespace SurveyBasket.Services.SurveyQuestions;

public class SurveyQuestionService(ISurveyQuestionRepository questionsRepo, ISurveyRepository surveyReop, IUserSubmissionsRepository submissionRepo) : ISurveyQuestionService
{
    public async Task<Result<SurveyQuestionResponse>> AddAsync(int surveyId, CreateSurveyQuestionRequest request, CancellationToken token = default)
    {
        if (!await surveyReop.ExistByIdAsync(surveyId, token))
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.SurveyNotFound());

        if (await questionsRepo.IsDuplicateSurveyQuestionAsync(request.Content, token))
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.ConflictSurveyQuestion());

        var question = request.Adapt<SurveyQuestion>();
        question.SurveyId = surveyId;

        var created = await questionsRepo.AddAsync(question, token);
        if (created is null)
            return Result.Failure<SurveyQuestionResponse>(SystemError.Database());

        var fullSurveyQuestion = await questionsRepo.GetQuestionWithOptionsAsync(surveyId, created.Id, token);

        return Result.Success<SurveyQuestionResponse>(fullSurveyQuestion.Adapt<SurveyQuestionResponse>());

    }
    public async Task<Result<ICollection<SurveyQuestionResponse>>> GetAllAsync(int surveyId, CancellationToken token = default)
    {
        if (!(await surveyReop.ExistByIdAsync(surveyId, token)))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyQuestionError.SurveyNotFound());
        ICollection<SurveyQuestion>? questions = await questionsRepo.GetAllAsync(surveyId, token);
        if (questions is null)
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SystemError.Database());
        return Result.Success<ICollection<SurveyQuestionResponse>>(questions.Adapt<ICollection<SurveyQuestionResponse>>());
    }
    public async Task<Result<SurveyQuestionResponse>> GetByIdAsync(int surveyId, int questionId, CancellationToken token = default)
    {
        if (!await surveyReop.ExistByIdAsync(surveyId, token))
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.SurveyNotFound());
        if (!await questionsRepo.ExistByIdAsync(surveyId, questionId, token))
            return Result.Failure<SurveyQuestionResponse>(SurveyQuestionError.SurveyQuestionNotFound());

        SurveyQuestion? question = await questionsRepo.GetQuestionWithOptionsAsync(surveyId: surveyId, questionId: questionId, token);
        if (question is null)
            return Result.Failure<SurveyQuestionResponse>(SystemError.Database());
        return Result.Success<SurveyQuestionResponse>(question.Adapt<SurveyQuestionResponse>());


    }
    public async Task<Result> RestoreSurveyQuestion(int surveyId, int questionId, CancellationToken token = default)
    {

        if (!await surveyReop.ExistByIdAsync(surveyId, token))
            return Result.Failure(SurveyQuestionError.SurveyNotFound());
        if (!await questionsRepo.ExistByIdAsyncIgnoredFilter(surveyId, questionId, token))
            return Result.Failure(SurveyQuestionError.SurveyQuestionNotFound());

        bool Restored = await questionsRepo.RestoreSurveyQuestion(surveyId: surveyId, questionId: questionId, token);
        if (!Restored)
            return Result.Failure(SystemError.Database());

        return Result.Success();
    }
    public async Task<Result> DeleteSurveyQuestionAsync(int surveyId, int questionId, CancellationToken token = default)
    {
        if (!await surveyReop.ExistByIdAsync(surveyId, token))
            return Result.Failure(SurveyQuestionError.SurveyNotFound());

        if (!await questionsRepo.ExistByIdAsync(surveyId, questionId, token))
            return Result.Failure(SurveyQuestionError.SurveyQuestionNotFound());

        bool deleted = await questionsRepo.DeleteSurveyQuestionAsync(surveyId, questionId, token);
        if (!deleted)
            return Result.Failure(SystemError.Database());

        return Result.Success();
    }
    public async Task<Result> UpdateSurveyQuestionAsync(int surveyId, int questionId, UpdateSurveyQuestionRequest updateRequest, CancellationToken token = default)
    {
        if (!await surveyReop.ExistByIdAsync(surveyId, token))
            return Result.Failure(SurveyQuestionError.SurveyNotFound());

        if (!await questionsRepo.ExistByIdAsync(surveyId, questionId, token))
            return Result.Failure(SurveyQuestionError.SurveyQuestionNotFound());

        if (await questionsRepo.ExistByContentWithDifferentId(surveyId, questionId, updateRequest.Content, token))
            return Result.Failure(SurveyQuestionError.ConflictSurveyQuestion());

        SurveyQuestion question = updateRequest.Adapt<SurveyQuestion>();
        bool updated = await questionsRepo.UpdateAsync(surveyId, questionId, question, token);
        if (!updated)
            return Result.Failure(SystemError.Database());

        return Result.Success();
    }

    public async Task<Result<ICollection<SurveyQuestionResponse>>> GetAvailableQuestionAsync(
    int surveyId, string userId, CancellationToken token)
    {
        if (await submissionRepo.SubmittedBeforeAsync(userId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(UserSubmissionError.DuplicateSubmission());

        if (!await surveyReop.ExistByIdAsync(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotFound());


        if (await surveyReop.IsSurveyNotStarted(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("The survey has not started yet."));


        if (await surveyReop.IsSurveyClosed(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.AlreadyClosed());


        if (!await surveyReop.IsSurveyAvailable(surveyId, token))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("The survey is not available or not published."));


        var questions = await questionsRepo.GetAvailableQuestionAsync(surveyId, token);
        var response = questions.Adapt<ICollection<SurveyQuestionResponse>>();

        return Result.Success(response);
    }

}

