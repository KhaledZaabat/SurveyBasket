using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities;

public class Survey : IAuditable, ISoftDeletable
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public PublishStatus Status { get; set; } = default!;

    public DateOnly StartsAt { get; set; }
    public DateOnly EndsAt { get; set; }

    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? UpdatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public ApplicationUser CreatedBy { get; set; } = default!;
    public ApplicationUser? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public string? DeletedById { get; set; }
    public DateTime? DeletedOn { get; set; }
    public ApplicationUser? DeletedBy { get; set; }

    public ICollection<SurveyQuestion> SurveyQuestions { get; set; }

}


