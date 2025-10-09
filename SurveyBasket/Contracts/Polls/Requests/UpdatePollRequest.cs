namespace SurveyBasket.Contracts.Polls.Requests;

public record UpdatePollRequest(
    string Title,
    string Summary,
    DateOnly StartsAt,
    DateOnly EndsAt
);


