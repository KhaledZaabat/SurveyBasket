namespace SurveyBasket.Contracts.Polls.Requests;

public record CreateSurveyRequest(
    string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
);