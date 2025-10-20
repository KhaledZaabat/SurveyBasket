using DotNetEnv;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Seeding;
using Serilog;
using SurveyBasket.Seeding;
using SurveyBasket.Services.Notifications;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();


builder.Services.AddApplicationServices(builder.Configuration);


builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(builder.Configuration));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/jobs", new DashboardOptions
    {
        Authorization =
        [
            new HangfireCustomBasicAuthenticationFilter
            {
                User = app.Configuration["HangfireSettings:Username"],
                Pass = app.Configuration["HangfireSettings:Password"]
            }
        ],
        DashboardTitle = "Survey Basket Dashboard"
    });
}



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        AppDbContext context = services.GetRequiredService<AppDbContext>();
        context.DisableAuditing = true;
        context.DisableSoftDeletion = true;
        await RoleSeeder.SeedRolesAsync(services);
        await UserSeeder.SeedUserAsync(services);
        await RoleClaimSeeder.SeedRoleClaimsAsync(services);
        context.DisableAuditing = false;
        context.DisableSoftDeletion = false;
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding initial data.");
    }
}



using (var scope = app.Services.CreateScope())
{
    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();


    RecurringJob.AddOrUpdate(
        "DailySurveysNotifications",
        () => notificationService.SendNewPollsNotification(null),
        "0 0 * * *"
    );
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();
