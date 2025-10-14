namespace SurveyBasket.Services.UserSubmissionServices;



public class UserSubmissionService(IUserSubmissionsRepository submissionRepo, ISurveyRepository surveyReop, ISurveyQuestionRepository questionRepo) : IUserSubmissionService
{
    public async Task<Result> AddAsync(int surveyId, string userId, UserSubmissionRequest request, CancellationToken cancellationToken = default)
    {

        if (await submissionRepo.SubmittedBeforeAsync(surveyId, userId, cancellationToken))
            return Result.Failure(UserSubmissionError.DuplicateSubmission());


        if (!await surveyReop.ExistByIdAsync(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotFound());


        if (await surveyReop.IsSurveyNotStarted(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("The survey has not started yet."));


        if (await surveyReop.IsSurveyClosed(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.AlreadyClosed());
        if (!await surveyReop.IsSurveyAvailable(surveyId, cancellationToken))
            return Result.Failure<ICollection<SurveyQuestionResponse>>(SurveyError.NotOpened("The survey is not available or not published."));

        ICollection<int> QuestionIds = request.submissionDetails.Select(d => d.QuestionId).ToList();
        ICollection<SurveyQuestion> questions = await questionRepo.GetAvailableQuestionAsync(surveyId, cancellationToken);
        if (!questions.Select(q => q.Id).SequenceEqual(QuestionIds))
            return Result.Failure(UserError.InvalidSubmission("U did Not filled the required Questions"));
        var validPairs = questions
            .SelectMany(q => q.SurveyOptions.Select(o => (q.Id, o.Id)))
            .ToHashSet();

        bool isValidSubmission =
            questions.Count == request.submissionDetails.Count &&
            request.submissionDetails.All(s => validPairs.Contains((s.QuestionId, s.OptionId)));

        if (!isValidSubmission)
            return Result.Failure(UserError.InvalidSubmission("Invalid or mismatched options."));
        UserSubmission submission = request.Adapt<UserSubmission>();
        submission.UserId = userId;
        submission.SurveyId = surveyId;
        submission.SubmittedOn = DateTime.UtcNow;
        await submissionRepo.AddAsync(submission, cancellationToken);
        return Result.Success();

    }
}
