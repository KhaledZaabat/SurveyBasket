namespace SurveyBasket.Domain.Entities;

public sealed class UserSubmission
{
    public int Id { get; set; }
    public int SurveyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;


    public Survey Survey { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public ICollection<SubmissionDetail> SubmissionDetails { get; set; } = [];
}