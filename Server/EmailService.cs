using System;
using System.Net;
using System.Net.Mail;


namespace ReichertsMeatDistributing.Server
{

    public class EmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            // Initialize SmtpClient with SMTP server settings
            _smtpClient = new SmtpClient("smtp.example.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("username", "password"),
                EnableSsl = true
            };
        }

        public void SendVerificationEmail(string userEmail, string verificationCode)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress("your-email@example.com");
            message.To.Add(new MailAddress(userEmail));
            message.Subject = "Password Reset Verification Code";
            message.Body = $"Your verification code is: {verificationCode}";

            _smtpClient.Send(message);
        }
    }

}
