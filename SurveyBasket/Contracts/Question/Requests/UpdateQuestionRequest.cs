namespace SurveyBasket.Contracts.Question.Requests;

public record UpdateQuestionRequest(
    string Content,
    List<string> Answers
);
