namespace SurveyBasket.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    string? DeletedById { get; set; }
    DateTime? DeletedOn { get; set; }
    ApplicationUser? DeletedBy { get; set; }



}