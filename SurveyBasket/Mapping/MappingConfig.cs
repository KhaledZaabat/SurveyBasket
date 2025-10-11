using Mapster;
using SurveyBasket.Contracts.Answers.Responses;
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

        config.NewConfig<AnswerResponse, Answer>()
            .Map(des => des.Content, src => src.Content)
            .Map(des => des.Id, src => src.Id)
            .Ignore(d => d.CreatedBy)
            .Ignore(d => d.CreatedById)
            .Ignore(d => d.CreatedOn)
            .Ignore(d => d.DeletedBy)
            .Ignore(d => d.DeletedById)
            .Ignore(d => d.DeletedOn)
            .Ignore(d => d.IsDeleted)
            .Ignore(d => d.QuestionId)
            .Ignore(d => d.UpdatedBy)
            .Ignore(d => d.UpdatedById)
            .Ignore(d => d.UpdatedOn).TwoWays();


        config.NewConfig<Question, QuestionResponse>()
            .Map(des => des.Content, src => src.Content)
            .Map(des => des.Id, src => src.Id)
            .Map(des => des.AnswerResponses, src => src.Answers)
            .TwoWays();
        config.NewConfig<CreateQuestionRequest, Question>()
              .Map(dest => dest.Answers,
                src => src.Answers.Select(a => new Answer { Content = a }).ToList());

    }
}


