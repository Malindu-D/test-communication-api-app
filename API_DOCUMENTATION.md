# Email Notification API - Documentation

## Overview

This is a .NET 8 Web API that sends email notifications using Azure Communication Service. It receives an email address via HTTP POST request and sends a formatted HTML email to that recipient.

---

## API Endpoints

### 1. **Send Email**

**Endpoint:** `POST /api/sendemail`

**Description:** Sends an email notification to the specified recipient.

**Request Body:**

```json
{
  "receiverEmail": "recipient@example.com"
}
```

**Success Response (200 OK):**

```json
{
  "success": true,
  "message": "Email sent successfully to recipient@example.com"
}
```

**Error Response (400 Bad Request):**

```json
{
  "success": false,
  "message": "Invalid email address format"
}
```

**Error Response (500 Internal Server Error):**

```json
{
  "success": false,
  "message": "Failed to send email. Please check logs for details."
}
```

---

### 2. **Health Check**

**Endpoint:** `GET /api/health`

**Description:** Checks if the API is running.

**Response (200 OK):**

```json
{
  "status": "healthy",
  "timestamp": "2025-11-19T14:30:00Z"
}
```

---

## Architecture

### **Controllers**

**EmailController.cs** - Handles HTTP requests for sending emails

```csharp
[HttpPost("sendemail")]
public async Task<ActionResult<EmailResponse>> SendEmail([FromBody] EmailRequest request)
{
    // 1. Validate email address
    if (!IsValidEmail(request.ReceiverEmail))
        return BadRequest("Invalid email address");

    // 2. Call email service to send email
    bool result = await _emailService.SendEmailAsync(request.ReceiverEmail);

    // 3. Return success or error response
    return result ? Ok(response) : StatusCode(500, error);
}
```

### **Services**

**AzureCommunicationEmailService.cs** - Sends emails using Azure Communication Service

```csharp
public async Task<bool> SendEmailAsync(string recipientEmail)
{
    // 1. Create email content with HTML template
    var emailContent = new EmailContent("User Data Notification")
    {
        Html = GenerateEmailHtml()
    };

    // 2. Create email message
    var emailMessage = new EmailMessage(
        senderAddress: _senderEmail,
        recipientAddress: recipientEmail,
        content: emailContent
    );

    // 3. Send email via Azure Communication Service
    EmailSendOperation result = await _emailClient.SendAsync(
        WaitUntil.Completed,
        emailMessage
    );

    return result.Value.Status == EmailSendStatus.Succeeded;
}
```

### **Models**

**EmailRequest.cs** - Request data model

```csharp
public class EmailRequest
{
    public string ReceiverEmail { get; set; }
}
```

**EmailResponse.cs** - Response data model

```csharp
public class EmailResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
}
```

---

## Configuration

### **Environment Variables (Azure App Service)**

The API requires these environment variables to be configured:

| Name                                          | Description                                   | Example                               |
| --------------------------------------------- | --------------------------------------------- | ------------------------------------- |
| `AzureCommunicationService__ConnectionString` | Azure Communication Service connection string | `endpoint=https://...;accesskey=...`  |
| `AzureCommunicationService__SenderEmail`      | Verified sender email address                 | `DoNotReply@yourdomain.azurecomm.net` |

**Note:** Use double underscore `__` for nested configuration in environment variables.

### **appsettings.json (Local Development)**

```json
{
  "AzureCommunicationService": {
    "ConnectionString": "YOUR_CONNECTION_STRING",
    "SenderEmail": "DoNotReply@yourdomain.com"
  }
}
```

---

## Email Template

The API sends a professional HTML email with:

- **Header:** "User Data Notification" with gradient background
- **Content:** Greeting and notification message
- **Data Table:** Shows notification details (type, date, time, status)
- **Footer:** Automated message disclaimer

**Sample Email Output:**

```
┌─────────────────────────────────────┐
│   📧 User Data Notification         │
│   (Blue gradient header)            │
├─────────────────────────────────────┤
│ Hello!                              │
│                                     │
│ This is an automated notification  │
│ containing user data information.  │
│                                     │
│ ┌─────────────────────────────────┐│
│ │ Field          | Value          ││
│ ├─────────────────────────────────││
│ │ Type           | User Data      ││
│ │ Date           | Nov 19, 2025   ││
│ │ Time           | 14:30:00 UTC   ││
│ │ Status         | ✅ Active      ││
│ └─────────────────────────────────┘│
│                                     │
│ © 2025 Email Notification System   │
└─────────────────────────────────────┘
```

---

## Dependencies

**NuGet Packages:**

```xml
<PackageReference Include="Azure.Communication.Email" Version="1.1.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.20" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

---

## CORS Configuration

The API allows cross-origin requests from any origin:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStaticWebApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

---

## Testing with Postman

### **Test 1: Send Email**

1. **Method:** POST
2. **URL:** `https://your-api.azurewebsites.net/api/sendemail`
3. **Headers:**
   - `Content-Type: application/json`
   - `Accept: application/json`
4. **Body (raw JSON):**
   ```json
   {
     "receiverEmail": "test@example.com"
   }
   ```

### **Test 2: Health Check**

1. **Method:** GET
2. **URL:** `https://your-api.azurewebsites.net/api/health`

---

## Deployment

### **GitHub Actions Workflow**

The API automatically deploys to Azure App Service when code is pushed to the `main` branch.

**Workflow Steps:**

1. Build .NET 8 application
2. Publish to artifact
3. Deploy to Azure Web App using Azure credentials

**Required GitHub Secrets:**

- `AZUREAPPSERVICE_CLIENTID_*`
- `AZUREAPPSERVICE_TENANTID_*`
- `AZUREAPPSERVICE_SUBSCRIPTIONID_*`

---

## Error Handling

The API includes comprehensive error handling:

```csharp
try
{
    // Send email logic
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error processing email request");
    return StatusCode(500, new EmailResponse
    {
        Success = false,
        Message = $"Error: {ex.Message}"
    });
}
```

**Logging includes:**

- Configuration validation
- Email send attempts
- Success/failure status
- Detailed error messages

---

## Security Considerations

✅ **Safe:**

- No hardcoded credentials in code
- Sensitive data stored in environment variables
- Connection strings not exposed in responses

⚠️ **Important:**

- Keep connection strings secret
- Don't commit `appsettings.Development.json` or `appsettings.Production.json`
- Verify sender email domain in Azure Communication Service

---

## Quick Start Guide

### **1. Clone Repository**

```bash
git clone <repository-url>
cd EmailNotificationApi
```

### **2. Configure Local Settings**

Edit `appsettings.json` with your Azure Communication Service credentials.

### **3. Run Locally**

```bash
dotnet restore
dotnet build
dotnet run
```

### **4. Test API**

Open browser: `https://localhost:7073/swagger`

### **5. Deploy to Azure**

Push to `main` branch - GitHub Actions will auto-deploy.

---

## Support

For issues or questions, check:

- Azure Communication Service documentation
- API logs in Azure App Service
- GitHub Actions workflow runs
