using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities;

public class Poll : IAuditable
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public bool IsPublished { get; set; }

    public DateOnly StartsAt { get; set; }
    public DateOnly EndsAt { get; set; }

    public string CreatedById { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? UpdatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }

    public ApplicationUser CreatedBy { get; set; } = default!;
    public ApplicationUser? UpdatedBy
    {
        get; set;

    }

}


