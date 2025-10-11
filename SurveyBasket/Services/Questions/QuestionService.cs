using Mapster;
using SurveyBasket.Contracts.Question.Requests;
using SurveyBasket.Shared.Errors;

namespace SurveyBasket.Services.Questions;

public class QuestionService(IQuestionRepository repo) : IQuestionService
{
    public async Task<Result<QuestionResponse>> AddAsync(int id, CreateQuestionRequest request)
    {
        if (!await repo.PollExistsAsync(id))
            return Result.Failure<QuestionResponse>(QuestionError.PoolNotFound());

        if (await repo.IsDuplicateQuestionAsync(request.Content))
            return Result.Failure<QuestionResponse>(QuestionError.ConflictQuestion());

        var question = request.Adapt<Question>();
        question.PollId = id;

        var created = await repo.AddAsync(question);
        if (created is null)
            return Result.Failure<QuestionResponse>(QuestionError.CreationFailed());

        var fullQuestion = await repo.GetWithAnswersAsync(created.Id);

        return Result.Success<QuestionResponse>(fullQuestion.Adapt<QuestionResponse>());

    }
}

