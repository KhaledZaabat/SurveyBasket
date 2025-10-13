using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities;

public sealed class UserSubmission : ISoftDeletable
{

    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;


    public Survey Survey { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public ICollection<SubmissionDetail> SubmissionDetails { get; set; } = [];

    public bool IsDeleted { get; set; }
    public string? DeletedById { get; set; }
    public DateTime? DeletedOn { get; set; }
    public ApplicationUser? DeletedBy { get; set; }

}