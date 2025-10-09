using SurveyBasket.Contracts.Polls.Requests;
using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Services.PollServices;

public interface IPollService : IScopedService
{
    Task<List<PollResponse>> GetAll(CancellationToken token = default);
    Task<PollResponse?> GetById(int id, CancellationToken token = default);
    Task<PollResponse> Add(CreatePollRequest request, CancellationToken token = default);
    Task<bool> Update(int id, UpdatePollRequest request, CancellationToken token = default);
    Task<bool> Delete(int id, CancellationToken token = default);
    Task<bool> TogglePublish(int id, CancellationToken cancellationToken = default);

}