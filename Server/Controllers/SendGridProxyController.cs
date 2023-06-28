using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReichertsMeatDistributing.Shared;
using System;
using System.Net.Mail;

namespace ReichertsMeatDistributing.Server.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendGridProxyController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SendGridProxyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ContactModel contact)
        {
            try
            {
                string email = _configuration["MailSettings:Mail"];
                string displayName = _configuration["MailSettings:DisplayName"];
                string password = _configuration["MailSettings:Password"];
                string host = _configuration["MailSettings:Host"];
                int port = _configuration.GetValue<int>("MailSettings:Port");

                MailAddress from = new MailAddress(contact.Email, contact.Name);
                MailAddress to = new MailAddress("reichertsdistributinginc@gmail.com");
                string subject = "Testing";
                string body = contact.Message;

                SendEmail(subject, from, to, body, email, displayName, password, host, port);

                return Ok();
            }
            catch (Exception ex)
            {
                // Error handling
                return StatusCode(500);
            }
        }

        protected void SendEmail(string subject, MailAddress from, MailAddress to, string body,
            string email, string displayName, string password, string host, int port)
        {
            using (SmtpClient mailClient = new SmtpClient(host, port))
            {
                mailClient.UseDefaultCredentials = false;
                mailClient.Credentials = new System.Net.NetworkCredential(email, password);
                mailClient.EnableSsl = true;

                using (MailMessage msgMail = new MailMessage(from, to))
                {
                    msgMail.Subject = subject;
                    msgMail.Body = body;
                    msgMail.IsBodyHtml = true;
                    msgMail.From = new MailAddress(email, displayName);

                    mailClient.Send(msgMail);
                }
            }
        }
    }
}
