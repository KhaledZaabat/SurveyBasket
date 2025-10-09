namespace SurveyBasket.Domain.Common;

public interface IAuditable
{
    string CreatedById { get; set; }
    DateTime CreatedOn { get; set; }
    string? UpdatedById { get; set; }
    DateTime? UpdatedOn { get; set; }

    ApplicationUser CreatedBy { get; set; }
    ApplicationUser? UpdatedBy { get; set; }
}