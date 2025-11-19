# Quick Setup Guide

## What I've Created

✅ **A .NET 8 Web API application** that:

- Receives email addresses from your static web app
- Sends beautiful HTML emails using Azure Communication Service
- Has a light blue themed email template
- Includes proper error handling and logging

## Version Used

- **.NET 8.0.414** (latest installed on your system)

## Next Steps

### 1️⃣ Create Azure Communication Service (You haven't done this yet)

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"**
3. Search for **"Communication Services"**
4. Click **Create** and fill in:
   - **Subscription**: Your Azure subscription
   - **Resource Group**: Create new or use existing
   - **Resource Name**: Choose a unique name (e.g., `email-notification-service`)
   - **Region**: Choose closest to you
5. Click **Review + Create** → **Create**

### 2️⃣ Set Up Email Domain

After creating the Communication Service:

1. Go to your Communication Service resource
2. In left menu, find **Email** → **Provision domains**
3. Choose one option:

   **Option A: Azure Managed Domain (Easiest - Recommended for testing)**

   - Click **"Add domain"**
   - Select **"Azure managed domain"**
   - It will create a domain like: `xxxxxxxx.azurecomm.net`
   - Your sender email will be: `DoNotReply@xxxxxxxx.azurecomm.net`
   - ✅ **Instantly ready to use!**

   **Option B: Custom Domain (For production)**

   - Requires DNS verification
   - Use your own domain (e.g., `notifications@yourdomain.com`)

4. Once domain is ready, click **"Connect domain"** to link it to your Communication Service

### 3️⃣ Get Connection String

1. In your Communication Service resource
2. Go to **Settings** → **Keys**
3. Copy the **Connection string** (Primary or Secondary)
4. It looks like: `endpoint=https://your-name.communication.azure.com/;accesskey=xxxxx`

### 4️⃣ Configure the API

1. Open: `EmailNotificationApi\appsettings.json`
2. Replace these values:

```json
{
  "AzureCommunicationService": {
    "ConnectionString": "PASTE_YOUR_CONNECTION_STRING_HERE",
    "SenderEmail": "DoNotReply@xxxxxxxx.azurecomm.net"
  }
}
```

### 5️⃣ Test Locally

```powershell
# Navigate to the API folder
cd EmailNotificationApi

# Run the application
dotnet run
```

**The API will start** and show you URLs like:

```
Now listening on: https://localhost:7123
Now listening on: http://localhost:5234
```

### 6️⃣ Test the Email Endpoint

Open a **new PowerShell window** and run:

```powershell
# Replace with your actual port from step 5
# Replace with your real email to receive the test
$body = @{
    receiverEmail = "YOUR_EMAIL@gmail.com"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:7123/api/sendemail" `
    -Method Post `
    -Body $body `
    -ContentType "application/json" `
    -SkipCertificateCheck
```

**You should receive a beautiful light blue themed email!** 📧

### 7️⃣ Connect to Your Static Web App

#### For Local Testing:

1. Keep the API running (from step 5)
2. Note the HTTPS URL (e.g., `https://localhost:7123`)
3. Update your static app's `/api/config/index.js`:
   ```javascript
   apiBaseEndpoint: "https://localhost:7123";
   ```

#### For Production (After deploying API to Azure):

1. Deploy the API to Azure App Service
2. Get the production URL (e.g., `https://your-api.azurewebsites.net`)
3. In your Static Web App configuration, add environment variable:
   - **Name**: `API_BASE_ENDPOINT`
   - **Value**: `https://your-api.azurewebsites.net`

## 📁 Project Structure Created

```
EmailNotificationApi/
├── Controllers/
│   └── EmailController.cs          # API endpoints (/api/sendemail)
├── Models/
│   ├── EmailRequest.cs            # Input model
│   └── EmailResponse.cs           # Output model
├── Services/
│   ├── IEmailService.cs           # Interface
│   └── AzureCommunicationEmailService.cs  # Email sending logic
├── appsettings.json               # ⚠️ UPDATE THIS with your Azure credentials
├── Program.cs                     # App configuration
└── README.md                      # Full documentation
```

## 🎨 Email Template Features

Your email includes:

- ✅ Light blue gradient header (as requested)
- ✅ Professional responsive design
- ✅ Data table with notification details
- ✅ Timestamp (UTC)
- ✅ Status indicators
- ✅ Information boxes
- ✅ Professional footer

## ❓ Common Issues

**"Connection string not configured"**
→ Update `appsettings.json` with your Azure connection string

**"Sender email is not configured"**
→ Update `appsettings.json` with your verified sender email

**Email not received**
→ Check spam folder
→ Verify domain is connected in Azure Communication Service
→ Check API logs for errors

## 🚀 Ready to Deploy?

See the full `README.md` for deployment instructions to:

- Azure App Service
- Azure Container Apps
- Other hosting options

---

**Need help?** Check the detailed `README.md` in the EmailNotificationApi folder!
