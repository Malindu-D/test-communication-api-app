using Azure;
using Azure.Communication.Email;

namespace EmailNotificationApi.Services;

public class AzureCommunicationEmailService : IEmailService
{
    private readonly EmailClient _emailClient;
    private readonly string _senderEmail;
    private readonly ILogger<AzureCommunicationEmailService> _logger;

    public AzureCommunicationEmailService(
        IConfiguration configuration,
        ILogger<AzureCommunicationEmailService> logger)
    {
        _logger = logger;
        
        var connectionString = configuration["AzureCommunicationService:ConnectionString"];
        var senderEmail = configuration["AzureCommunicationService:SenderEmail"];
        
        _logger.LogInformation("=== Azure Communication Service Configuration ===");
        _logger.LogInformation("ConnectionString exists: {HasConnectionString}", !string.IsNullOrEmpty(connectionString));
        _logger.LogInformation("SenderEmail exists: {HasSenderEmail}", !string.IsNullOrEmpty(senderEmail));
        _logger.LogInformation("SenderEmail value: {SenderEmail}", senderEmail ?? "NULL");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("Azure Communication Service connection string is not configured!");
            throw new InvalidOperationException("Azure Communication Service connection string is not configured. Please set AzureCommunicationService__ConnectionString in environment variables.");
        }
        
        if (string.IsNullOrEmpty(senderEmail))
        {
            _logger.LogError("Sender email is not configured!");
            throw new InvalidOperationException("Sender email is not configured. Please set AzureCommunicationService__SenderEmail in environment variables.");
        }
        
        _senderEmail = senderEmail;

        try
        {
            _emailClient = new EmailClient(connectionString);
            _logger.LogInformation("EmailClient created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create EmailClient. Check if connection string is valid.");
            throw;
        }
    }

    public async Task<bool> SendEmailAsync(string recipientEmail)
    {
        try
        {
            _logger.LogInformation("Preparing to send email to {RecipientEmail}", recipientEmail);

            var emailContent = new EmailContent("User Data Notification")
            {
                Html = GenerateEmailHtml()
            };

            var emailMessage = new EmailMessage(
                senderAddress: _senderEmail,
                recipientAddress: recipientEmail,
                content: emailContent);

            _logger.LogInformation("Sending email via Azure Communication Service...");

            EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                WaitUntil.Completed,
                emailMessage);

            _logger.LogInformation("Email sent successfully. Status: {Status}, MessageId: {MessageId}",
                emailSendOperation.Value.Status,
                emailSendOperation.Id);

            return emailSendOperation.Value.Status == EmailSendStatus.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {RecipientEmail}", recipientEmail);
            return false;
        }
    }

    private string GenerateEmailHtml()
    {
        return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f0f8ff;
            margin: 0;
            padding: 20px;
        }
        .email-container {
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }
        .header {
            background: linear-gradient(135deg, #87ceeb 0%, #4682b4 100%);
            color: white;
            padding: 30px 20px;
            text-align: center;
        }
        .header h1 {
            margin: 0;
            font-size: 28px;
            font-weight: 600;
        }
        .content {
            padding: 30px 20px;
        }
        .greeting {
            font-size: 18px;
            color: #333;
            margin-bottom: 20px;
        }
        .message {
            font-size: 16px;
            line-height: 1.6;
            color: #555;
            margin-bottom: 25px;
        }
        .info-box {
            background-color: #e6f7ff;
            border-left: 4px solid #4682b4;
            padding: 15px;
            margin: 20px 0;
            border-radius: 5px;
        }
        .info-box p {
            margin: 0;
            color: #333;
        }
        .data-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
        }
        .data-table th {
            background-color: #87ceeb;
            color: white;
            padding: 12px;
            text-align: left;
            font-weight: 600;
        }
        .data-table td {
            padding: 10px 12px;
            border-bottom: 1px solid #e0e0e0;
            color: #555;
        }
        .data-table tr:hover {
            background-color: #f5f5f5;
        }
        .footer {
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e0e0e0;
            color: #777;
            font-size: 14px;
        }
        .button {
            display: inline-block;
            background-color: #4682b4;
            color: white;
            padding: 12px 30px;
            text-decoration: none;
            border-radius: 5px;
            margin: 10px 0;
            font-weight: 600;
        }
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>📧 User Data Notification</h1>
        </div>
        <div class='content'>
            <p class='greeting'>Hello!</p>
            <p class='message'>
                This is an automated notification containing user data information. 
                Please review the details below:
            </p>
            
            <div class='info-box'>
                <p><strong>ℹ️ Important:</strong> This email was sent from our automated notification system.</p>
            </div>

            <table class='data-table'>
                <thead>
                    <tr>
                        <th>Field</th>
                        <th>Value</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td><strong>Notification Type</strong></td>
                        <td>User Data Report</td>
                    </tr>
                    <tr>
                        <td><strong>Date Sent</strong></td>
                        <td>" + DateTime.UtcNow.ToString("MMMM dd, yyyy") + @"</td>
                    </tr>
                    <tr>
                        <td><strong>Time Sent</strong></td>
                        <td>" + DateTime.UtcNow.ToString("HH:mm:ss") + @" UTC</td>
                    </tr>
                    <tr>
                        <td><strong>Status</strong></td>
                        <td>✅ Active</td>
                    </tr>
                    <tr>
                        <td><strong>Priority</strong></td>
                        <td>Normal</td>
                    </tr>
                </tbody>
            </table>

            <p class='message'>
                If you have any questions or concerns about this notification, please contact our support team.
            </p>
        </div>
        <div class='footer'>
            <p>© 2025 Email Notification System. All rights reserved.</p>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
    }
}
