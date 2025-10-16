using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.Settings;

namespace SurveyBasket.Services;

public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Username));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        _logger.LogInformation("Sending email to {email}", email);

        // Connect to Mailtrap SMTP sandbox
        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);

        // Authenticate using Mailtrap SMTP credentials
        await smtp.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);

        // Send the email
        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);

        _logger.LogInformation("Email sent to {email}", email);
    }
}
