using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Persistence.Interceptors;
using System.Text;

namespace SurveyBasket.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
           .AddControllersConfiguration()
           .AddValidationConfiguration()
           .AddDatabaseConfiguration(configuration)
           .AddAssemblyScanningConfiguration()
           .AddIdentityConfiguration()
           .AddJwtConfiguration(configuration)
           .ConfigureMappings()
           .ConfigureProblems()
           .ConfigureCaching(configuration);
        return services;
    }

    // ------------------ CONTROLLERS ------------------
    private static IServiceCollection AddControllersConfiguration(this IServiceCollection services)
    {
        services.AddControllers();
        return services;
    }

    // ------------------ VALIDATION ------------------
    private static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateSurveyRequest>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    // ------------------ DATABASE ------------------
    private static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<AuditAbleInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>((sp, options) =>
    options
        .UseSqlServer(connectionString)

        .AddInterceptors(sp.GetRequiredService<AuditAbleInterceptor>(), sp.GetRequiredService<SoftDeleteInterceptor>())
);
        return services;
    }

    // ------------------ SCANNING ------------------
    private static IServiceCollection AddAssemblyScanningConfiguration(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssembliesOf(typeof(IScopedService))
            .AddClasses(c => c.AssignableTo<IScopedService>()).AsImplementedInterfaces().WithScopedLifetime()
            .AddClasses(c => c.AssignableTo<ITransientService>()).AsImplementedInterfaces().WithTransientLifetime()
            .AddClasses(c => c.AssignableTo<ISingletonService>()).AsImplementedInterfaces().WithSingletonLifetime()
        );

        return services;
    }

    // ------------------ IDENTITY ------------------
    private static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password requirements
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
            // Lockout
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    // ------------------ JWT AUTH ------------------
    private static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {


        services.AddOptions<JwtSettings>()
            .BindConfiguration(JwtSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();


        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
           ?? throw new InvalidOperationException($"{JwtSettings.SectionName} Section is missing in configuration.");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }
    private static IServiceCollection ConfigureMappings(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    private static IServiceCollection ConfigureProblems(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance =
                    $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
            };
        }).AddProblemDetails();
        return services;
    }
    private static IServiceCollection ConfigureCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "survey-basket:";
        });

        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            };
        });

        return services;
    }
}
