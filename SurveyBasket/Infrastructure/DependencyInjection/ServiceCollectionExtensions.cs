

namespace SurveyBasket.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IPollRepository, EFPollRepository>();

        return services;
    }
}

