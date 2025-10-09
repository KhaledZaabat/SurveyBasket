﻿namespace SurveyBasket.Contracts.Polls.Requests;

public record CreatePollRequest(
    string Title,
    string Summary,
    bool IsPublished,
    DateOnly StartsAt,
    DateOnly EndsAt
);