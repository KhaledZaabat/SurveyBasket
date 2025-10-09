using Mapster;
using SurveyBasket.Contracts.Polls.Requests;
using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Services.PollServices;

public class PollService(IPollRepository pollRepository) : IPollService
{
    public async Task<List<PollResponse>> GetAll(CancellationToken token = default)
    {
        List<Poll> polls = await pollRepository.GetAll(token);
        return polls.Adapt<List<PollResponse>>();
    }

    public async Task<PollResponse?> GetById(int id, CancellationToken token = default)
    {
        var poll = await pollRepository.GetById(id, token);
        return poll?.Adapt<PollResponse>();
    }

    public async Task<PollResponse> Add(CreatePollRequest request, CancellationToken token = default)
    {
        var poll = request.Adapt<Poll>();
        var addedPoll = await pollRepository.Add(poll, token);
        return addedPoll.Adapt<PollResponse>();
    }

    public async Task<bool> Update(int id, UpdatePollRequest request, CancellationToken token = default)
    {
        var poll = request.Adapt<Poll>();
        return await pollRepository.Update(id, poll, token);
    }

    public Task<bool> Delete(int id, CancellationToken token = default)
    {
        return pollRepository.Delete(id, token);
    }
    public Task<bool> TogglePublish(int id, CancellationToken cancellationToken = default)
    {
        return pollRepository.TogglePublish(id, cancellationToken);
    }

}