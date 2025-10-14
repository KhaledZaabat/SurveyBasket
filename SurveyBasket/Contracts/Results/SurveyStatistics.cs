namespace SurveyBasket.Contracts.Results;

public record SurveyStatistics(
    string Question,
    IEnumerable<AnswerStatistics> SelectedAnswers
);