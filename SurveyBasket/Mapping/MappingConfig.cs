using Mapster;

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
    }
}


