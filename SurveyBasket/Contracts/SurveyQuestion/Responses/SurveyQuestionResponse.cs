using SurveyBasket.Contracts.SurveyOptions.Responses;

namespace SurveyBasket.Contracts.SurveyQuestion.Responses;

public record SurveyQuestionResponse(int Id, string Content, ICollection<SurveyOptionResponse> AnswerResponses);

