# Google OAuth Setup Instructions

## Step 1: Create Google Cloud Project and OAuth Credentials

### A. Set up Google Cloud Project
1. Go to the [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project:
   - Click "Select a project" at the top
   - Click "New Project"
   - Enter project name (e.g., "Reicherts Admin Auth")
   - Click "Create"

### B. Configure OAuth Consent Screen
1. Go to "APIs & Services" > "OAuth consent screen"
2. Choose "External" user type and click "Create"
3. Fill in the required fields:
   - App name: "Reicherts Admin Panel"
   - User support email: your email
   - Developer contact information: your email
4. Click "Save and Continue"
5. On Scopes page, click "Save and Continue" (no additional scopes needed)
6. On Test users page, add both authorized emails:
   - `jordaanmartin.it@gmail.com`
   - `tami.r.bickel@gmail.com`
7. Click "Save and Continue"

### C. Create OAuth 2.0 Credentials
1. Go to "APIs & Services" > "Credentials"
2. Click "Create Credentials" > "OAuth 2.0 Client IDs"
3. Choose "Web application"
4. Set the name: "Reicherts Admin Auth"
5. Add authorized redirect URIs:
   - For development: `https://localhost:7194/api/admin/google-response`
   - For production: `https://yourdomain.com/api/admin/google-response`
   - **Important**: Use port 7194 as shown above
6. Click "Create"
7. **Copy the Client ID and Client Secret** - you'll need these next

## Step 2: Configure Your Application

1. Copy your Client ID and Client Secret from Google Cloud Console
2. Update `Server/appsettings.json`:
   ```json
   {
     "Authentication": {
       "Google": {
         "ClientId": "YOUR_GOOGLE_CLIENT_ID_HERE",
         "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET_HERE"
       }
     }
   }
   ```

## Step 3: Authorized Users (Already Configured)

The application is already configured to allow access for:
- `jordaanmartin.it@gmail.com`
- `tami.r.bickel@gmail.com`

These are set in `Server/Program.cs` in the Google OAuth configuration.

## Step 4: Test the Integration

1. Build and run the application
2. Navigate to `/login`
3. Click "Sign in with Google"
4. Complete the Google OAuth flow
5. You should be redirected to the admin panel

## Security Notes

- Only the email specified in `authorizedEmail` will be allowed access
- The Google OAuth credentials should be kept secure
- For production, consider using environment variables instead of storing credentials in appsettings.json

## Troubleshooting

- Ensure your redirect URI in Google Cloud Console matches exactly
- Check that the Google+ API is enabled
- Verify your authorized email is correct in the code
- Check browser console for any JavaScript errors
