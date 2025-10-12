

namespace SurveyBasket.Services.SurveyServices;

public interface ISurveyService : IScopedService
{
    Task<Result<ICollection<SurveyResponse>>> GetAllAsync(CancellationToken token = default);
    public Task<Result<ICollection<SurveyResponse>>> GetAllIncludingDeletedAsync(CancellationToken token = default);
    Task<Result<SurveyResponse>> GetByIdAsync(int id, CancellationToken token = default);
    Task<Result<SurveyResponse>> AddAsync(CreateSurveyRequest request, CancellationToken token = default);
    Task<Result> UpdateAsync(int id, UpdateSurveyRequest request, CancellationToken token = default);
    Task<Result> DeleteAsync(int id, CancellationToken token = default);
    Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default);

}