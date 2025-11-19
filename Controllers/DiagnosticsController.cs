using Microsoft.AspNetCore.Mvc;

namespace EmailNotificationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(IConfiguration configuration, ILogger<DiagnosticsController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("config-check")]
    public IActionResult CheckConfiguration()
    {
        var connectionString = _configuration["AzureCommunicationService:ConnectionString"];
        var senderEmail = _configuration["AzureCommunicationService:SenderEmail"];

        var connectionStringLength = connectionString?.Length ?? 0;
        var previewLength = Math.Min(20, connectionStringLength);
        
        return Ok(new
        {
            ConnectionStringExists = !string.IsNullOrEmpty(connectionString),
            ConnectionStringLength = connectionStringLength,
            ConnectionStringPreview = connectionString != null && connectionString.Length > 0 
                ? connectionString.Substring(0, previewLength) + "..." 
                : "NULL",
            SenderEmailExists = !string.IsNullOrEmpty(senderEmail),
            SenderEmail = senderEmail ?? "NULL",
            AllConfigKeys = _configuration.AsEnumerable()
                .Where(x => x.Key.Contains("Azure", StringComparison.OrdinalIgnoreCase))
                .Select(x => new { x.Key, HasValue = !string.IsNullOrEmpty(x.Value) })
                .ToList()
        });
    }
}
