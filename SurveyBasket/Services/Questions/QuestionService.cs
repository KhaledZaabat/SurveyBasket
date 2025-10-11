using Mapster;
using SurveyBasket.Contracts.Question.Requests;
using SurveyBasket.Shared.Errors;

namespace SurveyBasket.Services.Questions;

public class QuestionService(IQuestionRepository queestionsRepo, IPollRepository poolReop) : IQuestionService
{
    public async Task<Result<QuestionResponse>> AddAsync(int id, CreateQuestionRequest request, CancellationToken token = default)
    {
        if (!await poolReop.ExistById(id, token))
            return Result.Failure<QuestionResponse>(QuestionError.PoolNotFound());

        if (await queestionsRepo.IsDuplicateQuestionAsync(request.Content, token))
            return Result.Failure<QuestionResponse>(QuestionError.ConflictQuestion());

        var question = request.Adapt<Question>();
        question.PollId = id;

        var created = await queestionsRepo.AddAsync(question, token);
        if (created is null)
            return Result.Failure<QuestionResponse>(QuestionError.CreationFailed());

        var fullQuestion = await queestionsRepo.GetWithAnswersAsync(created.Id, token);

        return Result.Success<QuestionResponse>(fullQuestion.Adapt<QuestionResponse>());

    }
}

