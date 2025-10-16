using DotNetEnv;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Serilog;
using SurveyBasket.Services.Notifications;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
//builder.Services.AddAppServices();

builder.Services.AddApplicationServices(builder.Configuration);
//After Here you Can Override

builder.Host.UseSerilog
    ((context, configuration) =>
    configuration.ReadFrom.Configuration(builder.Configuration));


var app = builder.Build();




Console.WriteLine("username: " + app.Configuration.GetValue<string>("HangfireSettings:Username"));

Console.WriteLine("password " + app.Configuration.GetValue<string>("HangfireSettings:Password"));

if (app.Environment.IsDevelopment())
{

    app.UseHangfireDashboard("/jobs", new DashboardOptions
    {
        Authorization =
       [
           new HangfireCustomBasicAuthenticationFilter
        {
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }
       ],
        DashboardTitle = "Survey Basket Dashboard",
        //IsReadOnlyFunc = (DashboardContext conext) => true
    });


}

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await RoleSeeder.SeedRolesAsync(services);
//} Later We add roles based


using (var scope = app.Services.CreateScope())
{
    INotificationService notificationService = scope.ServiceProvider.GetService<INotificationService>()!;
    RecurringJob.AddOrUpdate("DailySurveysNotifications",
        () => notificationService.SendNewPollsNotification(null), "0 0 * * *");// every day  at 00


}
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();
