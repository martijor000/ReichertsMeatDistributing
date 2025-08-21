# 🚀 Environment Variables Setup (Free & Simple!)

## **How It Works:**
1. **Environment Variables** (highest priority) - for production
2. **Configuration Files** (fallback) - for development

## **For Production (Recommended):**

### **Windows Server:**
```powershell
# Set environment variables
[Environment]::SetEnvironmentVariable("GOOGLE_CLIENT_ID", "your_client_id", "Machine")
[Environment]::SetEnvironmentVariable("GOOGLE_CLIENT_SECRET", "your_client_secret", "Machine")
[Environment]::SetEnvironmentVariable("EMAIL_PASSWORD", "your_email_password", "Machine")

# Restart your app after setting these
```

### **Linux/Mac Server:**
```bash
# Add to /etc/environment or ~/.bashrc
export GOOGLE_CLIENT_ID="your_client_id"
export GOOGLE_CLIENT_SECRET="your_client_secret"
export EMAIL_PASSWORD="your_email_password"

# Or set them before running your app
GOOGLE_CLIENT_ID="your_client_id" GOOGLE_CLIENT_SECRET="your_client_secret" dotnet run
```

## **For Local Development:**
Just use your `appsettings.Development.json` file with real credentials.

## **Benefits:**
- ✅ **100% Free**
- ✅ **No Google Cloud billing**
- ✅ **Production ready**
- ✅ **Industry standard**
- ✅ **Easy to manage**
- ✅ **Secure**

## **That's It!**
No complex setup, no additional services, no costs. Just set environment variables and deploy!
