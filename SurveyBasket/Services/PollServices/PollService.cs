using Mapster;
using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Services.PollServices;

public class PollService(IPollRepository pollRepository) : IPollService
{
    public async Task<Result<List<PollResponse>>> GetAllAsync(CancellationToken token = default)
    {
        List<Poll> polls = await pollRepository.GetAll(token);


        return Result.Success<List<PollResponse>>(polls.Adapt<List<Poll>, List<PollResponse>>());
    }

    public async Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken token = default)
    {
        Poll? poll = await pollRepository.GetById(id, token);
        if (poll == null) return Result<PollResponse>.Failure<PollResponse>(PollError.NotFound());
        return Result.Success<PollResponse>(poll.Adapt<Poll, PollResponse>());
    }

    public async Task<Result<PollResponse>> AddAsync(CreatePollRequest request, CancellationToken token = default)
    {
        Poll poll = request.Adapt<CreatePollRequest, Poll>();
        Poll? addedPoll = await pollRepository.Add(poll, token);
        if (addedPoll is null) return Result.Failure<PollResponse>(PollError.Conflict());


        return Result.Success<PollResponse>(addedPoll!.Adapt<Poll, PollResponse>());
    }

    public async Task<Result> UpdateAsync(int id, UpdatePollRequest request, CancellationToken token = default)
    {
        Poll poll = request.Adapt<Poll>();
        UpdateResult result = await pollRepository.Update(id, poll, token);

        return result switch
        {
            UpdateResult.Success => Result.Success(),
            UpdateResult.NotFound => Result.Failure(PollError.NotFound()),
            UpdateResult.Conflict => Result.Failure(PollError.Conflict()),
            _ => Result.Failure(GenericError.Unknown())
        };
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken token = default)
    {
        bool Deleted = await pollRepository.Delete(id, token);
        if (!Deleted) return Result.Failure(PollError.NotFound());
        return Result.Success();
    }
    public async Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
    {
        PublishStatus ToggledStatus = await pollRepository.TogglePublish(id, cancellationToken);
        if (ToggledStatus is null) return Result.Failure(PollError.NotFound());
        return Result.Success();
    }

}
