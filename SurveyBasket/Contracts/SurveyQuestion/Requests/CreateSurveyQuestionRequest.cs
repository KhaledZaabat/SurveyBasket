namespace SurveyBasket.Contracts.SurveyQuestion.Requests;

public record CreateSurveyQuestionRequest(
    string Content,
    List<string> SurveyQuestions
);
