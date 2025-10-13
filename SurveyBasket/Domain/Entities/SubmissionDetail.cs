using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities;

public sealed class SubmissionDetail : ISoftDeletable
{
    public int Id { get; set; }
    public int UserSubmissionId { get; set; }
    public int QuestionId { get; set; }
    public int OptionId { get; set; }

    public UserSubmission Submission { get; set; } = default!;

    public SurveyOption Option { get; set; } = default!;

    public bool IsDeleted { get; set; }
    public string? DeletedById { get; set; }
    public DateTime? DeletedOn { get; set; }
    public ApplicationUser? DeletedBy { get; set; }

}
