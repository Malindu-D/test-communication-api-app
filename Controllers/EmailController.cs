using EmailNotificationApi.Models;
using EmailNotificationApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmailNotificationApi.Controllers;

[ApiController]
[Route("api")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailController> _logger;

    public EmailController(IEmailService emailService, ILogger<EmailController> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("sendemail")]
    public async Task<ActionResult<EmailResponse>> SendEmail([FromBody] EmailRequest request)
    {
        try
        {
            _logger.LogInformation("Received email request for: {ReceiverEmail}", request.ReceiverEmail);

            if (string.IsNullOrWhiteSpace(request.ReceiverEmail))
            {
                return BadRequest(new EmailResponse
                {
                    Success = false,
                    Message = "Receiver email address is required"
                });
            }

            // Validate email format
            if (!IsValidEmail(request.ReceiverEmail))
            {
                return BadRequest(new EmailResponse
                {
                    Success = false,
                    Message = "Invalid email address format"
                });
            }

            bool result = await _emailService.SendEmailAsync(request.ReceiverEmail);

            if (result)
            {
                return Ok(new EmailResponse
                {
                    Success = true,
                    Message = $"Email sent successfully to {request.ReceiverEmail}"
                });
            }
            else
            {
                return StatusCode(500, new EmailResponse
                {
                    Success = false,
                    Message = "Failed to send email. Please check logs for details."
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing email request");
            
            // Return more detailed error message
            var errorMessage = ex.InnerException != null 
                ? $"{ex.Message} Inner: {ex.InnerException.Message}" 
                : ex.Message;
            
            return StatusCode(500, new EmailResponse
            {
                Success = false,
                Message = $"Error: {errorMessage}"
            });
        }
    }

    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
