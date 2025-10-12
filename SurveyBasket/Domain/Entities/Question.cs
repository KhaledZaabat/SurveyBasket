using SurveyBasket.Domain.Common;

namespace SurveyBasket.Domain.Entities;

public class Question : IAuditable, ISoftDeletable
{


    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int PollId { get; set; }
    public Poll Poll { get; set; } = default!;
    public ICollection<Answer>? Answers { get; set; } = [];


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

}

