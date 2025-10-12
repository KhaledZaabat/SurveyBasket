
namespace SurveyBasket.Contracts.Polls.Responses;

public record SurveyResponse(
    int Id,
    string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
);


