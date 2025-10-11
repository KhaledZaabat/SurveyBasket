using SurveyBasket.Contracts.Answers.Responses;

namespace SurveyBasket.Contracts.Question.Responses;

public record QuestionResponse(int Id, string Content, ICollection<AnswerResponse> AnswerResponses);

