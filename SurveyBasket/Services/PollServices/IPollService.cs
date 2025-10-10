using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Services.PollServices;

public interface IPollService : IScopedService
{
    Task<Result<List<PollResponse>>> GetAll(CancellationToken token = default);
    Task<Result<PollResponse>> GetById(int id, CancellationToken token = default);
    Task<Result<PollResponse>> Add(CreatePollRequest request, CancellationToken token = default);
    Task<Result> Update(int id, UpdatePollRequest request, CancellationToken token = default);
    Task<Result> Delete(int id, CancellationToken token = default);
    Task<Result> TogglePublish(int id, CancellationToken cancellationToken = default);

}