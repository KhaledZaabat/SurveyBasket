using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SurveyBasket.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddControllersConfiguration()
            .AddValidationConfiguration()
            .AddDatabaseConfiguration(configuration)
            .AddAssemblyScanningConfiguration()
            .AddIdentityConfiguration()
            .AddJwtConfiguration(configuration);
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
        services.AddValidatorsFromAssemblyContaining<CreatePollRequest>();
        services.AddFluentValidationAutoValidation();
        return services;
    }

    // ------------------ DATABASE ------------------
    private static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
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
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing in configuration.");

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

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
}
