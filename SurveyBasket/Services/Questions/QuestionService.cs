using Mapster;
using SurveyBasket.Contracts.Question.Requests;
using SurveyBasket.Shared.Errors;

namespace SurveyBasket.Services.Questions;

public class QuestionService(IQuestionRepository questionsRepo, IPollRepository poolReop) : IQuestionService
{
    public async Task<Result<QuestionResponse>> AddAsync(int pollId, CreateQuestionRequest request, CancellationToken token = default)
    {
        if (!await poolReop.ExistByIdAsync(pollId, token))
            return Result.Failure<QuestionResponse>(QuestionError.PoolNotFound());

        if (await questionsRepo.IsDuplicateQuestionAsync(request.Content, token))
            return Result.Failure<QuestionResponse>(QuestionError.ConflictQuestion());

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        var created = await questionsRepo.AddAsync(question, token);
        if (created is null)
            return Result.Failure<QuestionResponse>(QuestionError.CreationFailed());

        var fullQuestion = await questionsRepo.GetWithAnswersAsync(created.Id, token);

        return Result.Success<QuestionResponse>(fullQuestion.Adapt<QuestionResponse>());

    }
    public async Task<Result<ICollection<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken token = default)
    {
        if (!(await poolReop.ExistByIdAsync(pollId, token)))
            return Result.Failure<ICollection<QuestionResponse>>(QuestionError.PoolNotFound());
        ICollection<Question> questions = await questionsRepo.GetAllAsync(pollId, token);
        return Result.Success<ICollection<QuestionResponse>>(questions.Adapt<ICollection<QuestionResponse>>());
    }
    public async Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken token = default)
    {
        if (!await poolReop.ExistByIdAsync(pollId, token))
            return Result.Failure<QuestionResponse>(QuestionError.PoolNotFound());
        Question? question = await questionsRepo.GetWithAnswersAsync(pollId: pollId, questionId: questionId, token);
        if (question is null)
            return Result.Failure<QuestionResponse>(QuestionError.QuestionNotFound());

        return Result.Success<QuestionResponse>(question.Adapt<QuestionResponse>());


    }

}

