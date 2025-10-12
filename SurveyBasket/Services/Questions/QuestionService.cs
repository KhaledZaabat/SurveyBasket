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
            return Result.Failure<QuestionResponse>(SystemError.Database());

        var fullQuestion = await questionsRepo.GetWithAnswersAsync(pollId, created.Id, token);

        return Result.Success<QuestionResponse>(fullQuestion.Adapt<QuestionResponse>());

    }
    public async Task<Result<ICollection<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken token = default)
    {
        if (!(await poolReop.ExistByIdAsync(pollId, token)))
            return Result.Failure<ICollection<QuestionResponse>>(QuestionError.PoolNotFound());
        ICollection<Question>? questions = await questionsRepo.GetAllAsync(pollId, token);
        if (questions is null)
            return Result.Failure<ICollection<QuestionResponse>>(SystemError.Database());
        return Result.Success<ICollection<QuestionResponse>>(questions.Adapt<ICollection<QuestionResponse>>());
    }
    public async Task<Result<QuestionResponse>> GetByIdAsync(int pollId, int questionId, CancellationToken token = default)
    {
        if (!await poolReop.ExistByIdAsync(pollId, token))
            return Result.Failure<QuestionResponse>(QuestionError.PoolNotFound());
        if (!await questionsRepo.ExistByIdAsync(pollId, questionId, token))
            return Result.Failure<QuestionResponse>(QuestionError.QuestionNotFound());

        Question? question = await questionsRepo.GetWithAnswersAsync(pollId: pollId, questionId: questionId, token);
        if (question is null)
            return Result.Failure<QuestionResponse>(SystemError.Database());
        return Result.Success<QuestionResponse>(question.Adapt<QuestionResponse>());


    }
    public async Task<Result> RestoreQuestion(int pollId, int questionId, CancellationToken token = default)
    {

        if (!await poolReop.ExistByIdAsync(pollId, token))
            return Result.Failure(QuestionError.PoolNotFound());
        if (!await questionsRepo.ExistByIdAsyncIgnoredFilter(pollId, questionId, token))
            return Result.Failure(QuestionError.QuestionNotFound());

        bool Restored = await questionsRepo.RestoreQuestion(pollId: pollId, questionId: questionId, token);
        if (!Restored)
            return Result.Failure(SystemError.Database());

        return Result.Success();
    }
    public async Task<Result> DeleteQuestionAsync(int pollId, int questionId, CancellationToken token = default)
    {
        if (!await poolReop.ExistByIdAsync(pollId, token))
            return Result.Failure(QuestionError.PoolNotFound());

        if (!await questionsRepo.ExistByIdAsync(pollId, questionId, token))
            return Result.Failure(QuestionError.QuestionNotFound());

        bool deleted = await questionsRepo.DeleteQuestionAsync(pollId, questionId, token);
        if (!deleted)
            return Result.Failure(SystemError.Database());

        return Result.Success();
    }
    public async Task<Result> UpdateQuestionAsync(int pollId, int questionId, UpdateQuestionRequest updateRequest, CancellationToken token = default)
    {
        if (!await poolReop.ExistByIdAsync(pollId, token))
            return Result.Failure(QuestionError.PoolNotFound());

        if (!await questionsRepo.ExistByIdAsync(pollId, questionId, token))
            return Result.Failure(QuestionError.QuestionNotFound());

        if (await questionsRepo.ExistByContentWithDifferentId(pollId, questionId, updateRequest.Content, token))
            return Result.Failure(QuestionError.ConflictQuestion());

        Question question = updateRequest.Adapt<Question>();
        bool updated = await questionsRepo.UpdateAsync(pollId, questionId, question, token);
        if (!updated)
            return Result.Failure(SystemError.Database());

        return Result.Success();
    }
}

