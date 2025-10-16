using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SurveyBasket.Services.Notifications;

public class NotificationService(
    ISurveyRepository surveyRepository,
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender) : INotificationService
{
    public async Task SendNewPollsNotification(int? surveyId = null)
    {
        IEnumerable<Survey> surveys;

        if (surveyId.HasValue)
        {
            var survey = await surveyRepository.GetByIdAsync(surveyId.Value);
            if (survey is null)
                return;

            surveys = new[] { survey };
        }
        else
        {
            surveys = await surveyRepository.GetPublishedTodaysSurveys();
        }

        var users = await userManager.Users.ToListAsync();
        string front_end = "my-front-end.com";

        foreach (var user in users)
        {
            foreach (var survey in surveys)
            {
                var placeholders = new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName },
                    { "{{surveyTitle}}", survey.Title },
                    { "{{endDate}}", survey.EndsAt.ToString("d") },
                    { "{{url}}", $"{front_end}/surveys/start/{survey.Id}" }
                };

                var body = HtmlBodyBuilder.GenerateEmailBody("SurveyNotification", placeholders);

                BackgroundJob.Enqueue<NotificationService>(
                    s => s.SendSurveyEmail(user.Email!, $"📣 Survey Basket: New Survey - {survey.Title}", body)
                );
            }
        }
    }

    public Task SendSurveyEmail(string email, string subject, string body)
        => emailSender.SendEmailAsync(email, subject, body);
}
