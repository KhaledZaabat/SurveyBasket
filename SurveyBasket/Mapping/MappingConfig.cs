using Mapster;
using SurveyBasket.Contracts.Question.Requests;

namespace SurveyBasket.Mapping;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreatePollRequest, Poll>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Summary, src => src.Summary)
            .Map(dest => dest.Status, src => new PublishStatus(src.IsPublished))
            .Map(dest => dest.StartsAt, src => src.StartsAt)
            .Map(dest => dest.EndsAt, src => src.EndsAt)
            .TwoWays();


        config.NewConfig<UpdatePollRequest, Poll>()
            .Map(dest => dest.Title, src => src.Title)
            .Map(dest => dest.Summary, src => src.Summary)
            .Ignore(dest => dest.Status)
            .Map(dest => dest.StartsAt, src => src.StartsAt)
            .Map(dest => dest.EndsAt, src => src.EndsAt)
            .TwoWays();

        config.NewConfig<Question, QuestionResponse>()
            .Map(des => des.Id, src => src.Id)
            .Map(des => des.Content, src => src.Content)
            .Map(des => des.AnswerResponses, src => src.Answers).TwoWays();


        config.NewConfig<CreateQuestionRequest, Question>()
              .Map(dest => dest.Answers,
                src => src.Answers.Select(a => new Answer { Content = a }).ToList());

    }
}


