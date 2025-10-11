using SurveyBasket.Contracts.Polls.Responses;

namespace SurveyBasket.Services.PollServices;

public interface IPollService : IScopedService
{
    Task<Result<List<PollResponse>>> GetAllAsync(CancellationToken token = default);
    Task<Result<PollResponse>> GetByIdAsync(int id, CancellationToken token = default);
    Task<Result<PollResponse>> AddAsync(CreatePollRequest request, CancellationToken token = default);
    Task<Result> UpdateAsync(int id, UpdatePollRequest request, CancellationToken token = default);
    Task<Result> DeleteAsync(int id, CancellationToken token = default);
    Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default);

}