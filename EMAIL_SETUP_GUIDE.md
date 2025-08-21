# Free Email Setup Guide for Contact Form

This guide shows you how to set up the contact form to send emails using your existing Gmail account **completely free**.

## What You Need
- Your existing Gmail account: `reichertsdistributinginc@gmail.com`
- 5 minutes to set up an "App Password"

## Step 1: Enable 2-Factor Authentication (if not already enabled)

1. Go to your Google Account settings: https://myaccount.google.com/
2. Click on "Security" in the left sidebar
3. Under "Signing in to Google", click "2-Step Verification"
4. Follow the steps to enable 2FA if it's not already enabled

## Step 2: Create an App Password

1. Still in Google Account settings, go to "Security"
2. Under "Signing in to Google", click "App passwords"
3. Select "Mail" for the app and "Other (Custom name)" for device
4. Type "Reicherts Website" as the custom name
5. Click "Generate"
6. **Copy the 16-character password that appears** (something like: `abcd efgh ijkl mnop`)

## Step 3: Add the Password to Your Website

1. Open `Server/appsettings.json` in your project
2. Find the "Email" section
3. Replace the empty `"FromPassword": ""` with your app password:
   ```json
   "FromPassword": "abcd efgh ijkl mnop"
   ```
   (Use the actual password you copied, without spaces)

## Step 4: Test the Contact Form

1. Run your website
2. Go to the Contact Us page
3. Fill out and submit the contact form
4. Check your Gmail inbox for the message

## How It Works

- **Completely Free**: Uses Gmail's free SMTP service
- **No Monthly Fees**: No third-party email services required
- **Professional**: Emails come from your business email
- **Secure**: Uses Google's secure email infrastructure
- **Reliable**: Gmail has 99.9% uptime

## What Happens When Someone Submits the Form

1. Customer fills out the contact form on your website
2. Email is sent to your Gmail inbox (`reichertsdistributinginc@gmail.com`)
3. The email includes all the customer's details
4. You can reply directly from Gmail to respond to the customer
5. The customer's email is automatically set as the "Reply-To" address

## Security Notes

- The app password is only used by your website
- It's separate from your main Gmail password
- You can revoke it anytime from Google Account settings
- It only has permission to send emails, nothing else

## Troubleshooting

If emails aren't sending:
1. Double-check the app password is correct (no spaces)
2. Make sure 2FA is enabled on your Google account
3. Check the server logs for error messages
4. Verify your Gmail account can send emails normally

**Cost: $0/month - Uses your existing Gmail account!**
