using Mapster;
using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Services.PollServices;

public class PollService(IPollRepository pollRepository) : IPollService
{
    public async Task<Result<List<PollResponse>>> GetAllAsync(CancellationToken token = default)
    {
        var polls = await pollRepository.GetAllAsync(token);
        return Result.Success(polls.Adapt<List<PollResponse>>());
    }

    public async Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken token = default)
    {
        var poll = await pollRepository.GetByIdAsync(id, token);
        if (poll is null)
            return Result.Failure<PollResponse>(PollError.NotFound());

        return Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result<PollResponse>> AddAsync(CreatePollRequest request, CancellationToken token = default)
    {


        if (await pollRepository.ExistByTitle(request.Title, token))
            return Result.Failure<PollResponse>(PollError.Conflict());

        var poll = request.Adapt<Poll>();
        var addedPoll = await pollRepository.AddAsync(poll, token);

        return Result.Success(addedPoll.Adapt<PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, UpdatePollRequest request, CancellationToken token = default)
    {
        var poll = await pollRepository.GetByIdAsync(id, token);
        if (poll is null)
            return Result.Failure(PollError.NotFound());



        if (await pollRepository.ExistByTitleWithDifferentId(request.Title, id, token))
            return Result.Failure(PollError.Conflict());

        // Apply updates
        poll.Title = request.Title;
        poll.Summary = request.Summary;
        poll.StartsAt = request.StartsAt;
        poll.EndsAt = request.EndsAt;

        await pollRepository.UpdateAsync(poll, token);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken token = default)
    {
        var poll = await pollRepository.GetByIdAsync(id, token);
        if (poll is null)
            return Result.Failure(PollError.NotFound());

        await pollRepository.DeleteAsync(poll, token);
        return Result.Success();
    }

    public async Task<Result> TogglePublishAsync(int id, CancellationToken token = default)
    {
        var poll = await pollRepository.GetByIdAsync(id, token);
        if (poll is null)
            return Result.Failure(PollError.NotFound());

        poll.Status = new PublishStatus(!poll.Status.IsPublished);
        await pollRepository.UpdateAsync(poll, token);

        return Result.Success();
    }
}
