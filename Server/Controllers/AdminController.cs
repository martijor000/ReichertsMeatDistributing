using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReichertsMeatDistributing.Server.Data;
using ReichertsMeatDistributing.Shared;
using System.Security.Cryptography;
using System.Text;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        private string GenerateSHA256Hash(string input, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input + salt);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }

        private bool CompareHash(string input, string storedSalt, string encrypted)
        {
            return GenerateSHA256Hash(input, storedSalt) == encrypted;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // For now, use hardcoded admin credentials
            // In production, you should store these in the database
            string adminUsername = "admin@reichertsdistributing.com";
            string adminPassword = "Admin123!";
            string salt = "defaultSalt";
            string expectedHash = GenerateSHA256Hash(adminPassword, salt);

            if (model.Username == adminUsername && 
                GenerateSHA256Hash(model.Password, salt) == expectedHash)
            {
                return Ok();
            }

            return Unauthorized();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            // For now, just return success if email matches admin email
            string adminEmail = "admin@reichertsdistributing.com";
            
            if (email == adminEmail)
            {
                // In a real application, you would send a reset email
                return Ok("Password reset email sent");
            }

            return NotFound();
        }

        [HttpGet("admin-email")]
        public IActionResult GetAdminEmail()
        {
            // Return the hardcoded admin email
            return Ok("admin@reichertsdistributing.com");
        }
    }
}

