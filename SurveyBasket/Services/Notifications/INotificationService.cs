namespace SurveyBasket.Services.Notifications;

public interface INotificationService : IScopedService
{
    Task SendNewPollsNotification(int? surveyId = null);
}