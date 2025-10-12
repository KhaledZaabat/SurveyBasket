namespace SurveyBasket.Domain.Entities;

public sealed class SubmissionDetail
{
    public int Id { get; set; }
    public int VoteId { get; set; }
    public int QuestionId { get; set; }
    public int AnswerId { get; set; }

    public UserSubmission Submission { get; set; } = default!;
    public SurveyQuestion Question { get; set; } = default!;
    public SurveyOption Option { get; set; } = default!;
}
