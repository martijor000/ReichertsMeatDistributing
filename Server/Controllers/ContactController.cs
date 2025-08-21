using Microsoft.AspNetCore.Mvc;
using ReichertsMeatDistributing.Shared;
using System.Net;
using System.Net.Mail;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IConfiguration configuration, ILogger<ContactController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendContactEmail([FromBody] ContactModel contact)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contact.Name) || 
                    string.IsNullOrWhiteSpace(contact.Email) || 
                    string.IsNullOrWhiteSpace(contact.Message))
                {
                    return BadRequest("All required fields must be filled.");
                }

                // Get email configuration from appsettings
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var fromEmail = _configuration["Email:FromEmail"] ?? "reichertsdistributinginc@gmail.com";
                var fromPassword = _configuration["Email:FromPassword"] ?? "";
                var toEmail = _configuration["Email:ToEmail"] ?? "reichertsdistributinginc@gmail.com";

                if (string.IsNullOrEmpty(fromPassword))
                {
                    _logger.LogWarning("Email password not configured. Email will not be sent.");
                    return StatusCode(500, "Email service not configured properly.");
                }

                // Create email message
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Reicherts Website Contact Form"),
                    Subject = $"New Contact Form Submission from {contact.Name}",
                    Body = $@"
New contact form submission received:

Name: {contact.Name}
Email: {contact.Email}
Phone: {contact.PhoneNumber ?? "Not provided"}
Message:
{contact.Message}

--
This message was sent from the Reicherts Meat Distributing website contact form.
Reply directly to this email to respond to the customer.",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);
                mailMessage.ReplyToList.Add(contact.Email);

                // Configure SMTP client
                using var smtpClient = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                // Send email
                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation($"Contact email sent successfully from {contact.Email}");
                
                return Ok(new { message = "Email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send contact email");
                return StatusCode(500, "Failed to send email. Please try again or contact us directly.");
            }
        }
    }
}
