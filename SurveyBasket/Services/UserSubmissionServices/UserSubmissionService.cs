using SurveyBasket.Shared.Errors;

namespace SurveyBasket.Services.UserSubmissionServices;

public class UserSubmissionService(
    IUserSubmissionsRepository submissionRepo,
    ISurveyRepository surveyRepo,
    ISurveyQuestionRepository questionRepo,
    ILogger<UserSubmissionService> logger) : IUserSubmissionService
{
    public async Task<Result> AddAsync(int surveyId, string userId, UserSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("User {UserId} submitting survey ID {SurveyId}", userId, surveyId);

        if (await submissionRepo.IsSubmittedBeforeAsync(surveyId, userId, cancellationToken))
            return Result.Failure(UserSubmissionError.DuplicateSubmission());

        if (!await surveyRepo.ExistByIdAsync(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotFound());

        if (await surveyRepo.IsSurveyNotStarted(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("The survey has not started yet."));

        if (await surveyRepo.IsSurveyClosed(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.AlreadyClosed());

        if (!await surveyRepo.IsSurveyAvailable(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("Survey not available or published."));

        ICollection<int> questionIds = request.submissionDetails.Select(d => d.QuestionId).ToList();
        var questions = await questionRepo.GetAvailableQuestionAsync(surveyId, cancellationToken);

        if (!questions.Select(q => q.Id).SequenceEqual(questionIds))
        {
            logger.LogWarning("User {UserId} submitted invalid question set for survey {SurveyId}", userId, surveyId);
            return Result.Failure(UserError.InvalidSubmission("You did not fill all required questions."));
        }

        var validPairs = questions
            .SelectMany(q => q.SurveyOptions.Select(o => (q.Id, o.Id)))
            .ToHashSet();

        bool isValidSubmission =
            questions.Count == request.submissionDetails.Count &&
            request.submissionDetails.All(s => validPairs.Contains((s.QuestionId, s.OptionId)));

        if (!isValidSubmission)
        {
            logger.LogWarning("User {UserId} submitted invalid/mismatched options for survey ID {SurveyId}", userId, surveyId);
            return Result.Failure(UserError.InvalidSubmission("Invalid or mismatched options."));
        }

        var submission = request.Adapt<UserSubmission>();
        submission.UserId = userId;
        submission.SurveyId = surveyId;
        submission.SubmittedOn = DateTime.UtcNow;

        await submissionRepo.AddAsync(submission, cancellationToken);

        logger.LogInformation("User {UserId} successfully submitted survey ID {SurveyId}", userId, surveyId);
        return Result.Success();
    }
}
