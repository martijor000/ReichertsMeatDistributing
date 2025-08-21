# Deployment Configuration Guide

## For Local Development
The application will automatically use `appsettings.Development.json` when running locally in development mode.

## For Production Deployment

### Option 1: Use the Deployment Script (Recommended)
1. Copy `deploy-production.ps1` to your production server
2. Run the script as Administrator: `.\deploy-production.ps1`
3. Restart your application

### Option 2: Manual Environment Variables
Set these environment variables on your production server:

```bash
# Google OAuth
GOOGLE_CLIENT_ID=your_google_client_id_here
GOOGLE_CLIENT_SECRET=your_google_client_secret_here

# Email Configuration
EMAIL_PASSWORD=your_email_app_password_here

# Google Cloud Project
GOOGLE_CLOUD_PROJECT_ID=your_google_cloud_project_id_here
```

### Option 3: Azure App Service Configuration
If using Azure, add these to your App Service Configuration:
- `GOOGLE_CLIENT_ID`
- `GOOGLE_CLIENT_SECRET`
- `EMAIL_PASSWORD`
- `GOOGLE_CLOUD_PROJECT_ID`

## Google Cloud Console Integration

The application now integrates with Google Cloud Console:
- **Automatic OAuth discovery** from Google's endpoints
- **Environment variable priority** over configuration files
- **Fallback to configuration** if environment variables aren't set
- **Production-ready** with secure credential management

## Security Notes
- **Never commit sensitive credentials** to Git
- **Use environment variables** in production for security
- **The `appsettings.Development.json` file** is excluded from Git
- **Google Cloud Console integration** provides additional security layers
