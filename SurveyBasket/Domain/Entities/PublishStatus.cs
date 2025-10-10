namespace SurveyBasket.Domain.Entities;


[Owned]
public record PublishStatus(bool IsPublished)
{
    public static PublishStatus Draft() => new(false);
    public static PublishStatus Published() => new(true);
    public bool CanBeEdited() => !IsPublished;
    public override string ToString() => IsPublished ? "Published" : "Draft";
}