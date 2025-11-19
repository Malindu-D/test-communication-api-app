# Email Notification API

A .NET 8 Web API application that receives email addresses and sends notifications using Azure Communication Service.

## Overview

This API works in conjunction with the Email Notification frontend application to send beautiful HTML emails with user data.

## Technology Stack

- **.NET Version**: 8.0.414
- **Framework**: ASP.NET Core Web API
- **Azure SDK**: Azure.Communication.Email 1.1.0

## Project Structure

```
EmailNotificationApi/
├── Controllers/
│   └── EmailController.cs        # API endpoints
├── Models/
│   ├── EmailRequest.cs           # Request model
│   └── EmailResponse.cs          # Response model
├── Services/
│   ├── IEmailService.cs          # Email service interface
│   └── AzureCommunicationEmailService.cs  # Azure email implementation
├── appsettings.json              # Configuration
└── Program.cs                    # Application startup
```

## API Endpoints

### 1. Send Email

**POST** `/api/sendemail`

Sends an email notification to the specified recipient.

**Request Body:**

```json
{
  "receiverEmail": "recipient@example.com"
}
```

**Success Response (200):**

```json
{
  "success": true,
  "message": "Email sent successfully to recipient@example.com"
}
```

**Error Response (400/500):**

```json
{
  "success": false,
  "message": "Error description"
}
```

### 2. Health Check

**GET** `/api/health`

Returns the health status of the API.

**Response (200):**

```json
{
  "status": "healthy",
  "timestamp": "2025-11-19T10:30:00Z"
}
```

## Setup Instructions

### Prerequisites

1. **.NET 8 SDK** installed
2. **Azure Communication Service** resource created
3. **Verified email domain** in Azure Communication Service

### Step 1: Create Azure Communication Service

1. Go to [Azure Portal](https://portal.azure.com)
2. Create a new **Azure Communication Service** resource
3. Once created, go to **Keys** section and copy the **Connection String**
4. Go to **Email** → **Domains** → Set up your domain (Azure managed or custom)
5. Note your verified sender email address (e.g., `DoNotReply@your-domain.azurecommailedomain.net`)

### Step 2: Configure the Application

1. Open `appsettings.json`
2. Replace the placeholder values:

```json
{
  "AzureCommunicationService": {
    "ConnectionString": "endpoint=https://your-resource.communication.azure.com/;accesskey=YOUR_KEY",
    "SenderEmail": "DoNotReply@your-verified-domain.azurecommailedomain.net"
  }
}
```

### Step 3: Build and Run

```powershell
# Navigate to the project directory
cd EmailNotificationApi

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The API will start on:

- HTTPS: `https://localhost:7XXX` (port may vary)
- HTTP: `http://localhost:5XXX` (port may vary)

Check the console output for the exact URLs.

## Email Template

The application sends a beautiful HTML email with:

- **Light blue gradient header** (as requested)
- Professional responsive design
- User data table with:
  - Notification Type
  - Date & Time Sent (UTC)
  - Status
  - Priority
- Information box with important notices
- Professional footer

## Testing the API

### Using Swagger UI

1. Run the application
2. Navigate to `https://localhost:XXXX/swagger` in your browser
3. Use the interactive UI to test endpoints

### Using PowerShell

```powershell
# Test the health endpoint
Invoke-RestMethod -Uri "https://localhost:7XXX/api/health" -Method Get

# Send an email (replace port and email)
$body = @{
    receiverEmail = "test@example.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7XXX/api/sendemail" `
    -Method Post `
    -Body $body `
    -ContentType "application/json"
```

### Using cURL

```bash
# Send email
curl -X POST "https://localhost:7XXX/api/sendemail" \
  -H "Content-Type: application/json" \
  -d '{"receiverEmail": "test@example.com"}'
```

## Integration with Static Web App

To connect your static web app to this API:

1. Deploy this API to Azure (App Service, Container Apps, etc.)
2. Get the public URL (e.g., `https://your-api.azurewebsites.net`)
3. In your Static Web App, set the environment variable:
   - **Key**: `API_BASE_ENDPOINT`
   - **Value**: `https://your-api.azurewebsites.net`

The static web app will automatically append `/api/sendemail` when the submit button is clicked.

## Deployment to Azure

### Option 1: Azure App Service

```powershell
# Publish the application
dotnet publish -c Release -o ./publish

# Deploy to Azure App Service (requires Azure CLI)
az webapp up --name your-api-name --resource-group your-rg
```

### Option 2: Azure Container Apps

1. Create a Dockerfile (if needed)
2. Build and push container image
3. Deploy to Azure Container Apps

### Option 3: Azure Functions (Alternative)

Convert to Azure Functions if you prefer serverless deployment.

## Environment Variables (Production)

For production deployment, use environment variables instead of appsettings.json:

- `AzureCommunicationService__ConnectionString`
- `AzureCommunicationService__SenderEmail`

## CORS Configuration

The API is configured to accept requests from any origin. For production, update the CORS policy in `Program.cs`:

```csharp
policy.WithOrigins("https://your-static-web-app.azurestaticapps.net")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

## Troubleshooting

### Email not sending

- Verify Azure Communication Service connection string
- Ensure sender email domain is verified
- Check Azure Communication Service quota/limits
- Review application logs for errors

### CORS errors

- Verify CORS policy includes your frontend URL
- Check browser console for specific error messages

### Connection errors

- Ensure the API is running
- Verify firewall/network settings
- Check SSL certificate if using HTTPS

## Support

For issues or questions:

1. Check application logs
2. Review Azure Communication Service status
3. Verify configuration in appsettings.json

## License

This project is created for demonstration purposes.
