namespace SurveyBasket.Contracts.Polls.Requests;

public record UpdateSurveyRequest(
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
);


