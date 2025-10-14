namespace SurveyBasket.Contracts.Results;

public record SubmissionsPerDayResponse(
    DateOnly Date,
    int NumberOfSubmissions
);