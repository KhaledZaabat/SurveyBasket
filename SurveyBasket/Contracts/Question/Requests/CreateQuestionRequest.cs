namespace SurveyBasket.Contracts.Question.Requests;

public record CreateQuestionRequest(
    string Content,
    List<string> Answers
);
