namespace SurveyBasket.Contracts.SurveyQuestion.Requests;

public record UpdateSurveyQuestionRequest(
    string Content,
    List<string> SurveyQuestions
);
