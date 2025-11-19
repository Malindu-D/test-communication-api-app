namespace EmailNotificationApi.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string recipientEmail);
}
