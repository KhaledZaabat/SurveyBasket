

namespace SurveyBasket.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IPollRepository, EFPollRepository>();

        return services;
    }
    public static IServiceCollection AddAutomaticAppServices(this IServiceCollection services)
    {
        services.Scan(scan => scan
      .FromAssembliesOf(typeof(IScopedService))
      .AddClasses(classes => classes.AssignableTo<IScopedService>())
          .AsImplementedInterfaces()
          .WithScopedLifetime()
      .AddClasses(classes => classes.AssignableTo<ISingletonService>())
          .AsImplementedInterfaces()
          .WithSingletonLifetime()
      .AddClasses(classes => classes.AssignableTo<ITransientService>())
          .AsImplementedInterfaces()
          .WithTransientLifetime()
  );
        return services;
    }
}

