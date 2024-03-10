using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ReichertsMeatDistributing.Server.Model;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly string _connectionString;

        public AdminController(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("Default");
        }

        private string GenerateSHA256Hash(string input, string salt)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input + salt);
            System.Security.Cryptography.SHA256Managed sha256hashstring =
                new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha256hashstring.ComputeHash(bytes);
            return Convert.ToHexString(hash);
        }

        private bool CompareHash(string input, string storedSalt, string encrypted)
        {
            if (GenerateSHA256Hash(input, storedSalt).ToString() == encrypted)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                string sqlCommand = "SELECT * FROM Admin WHERE Username = @Username";
                var admin = conn.QueryFirstOrDefault<Admin>(sqlCommand, new { Username = model.Username });

                if (admin != null && CompareHash(model.Password, admin.SaltHash, admin.PasswordHash))
                {
                    // Passwords match, login successful
                    return Ok();
                }
            }

            // Invalid username or password
            return Unauthorized();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] string email)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                string sqlCommand = "SELECT * FROM Admin WHERE Email = @Email";
                var admin = conn.QueryFirstOrDefault<Admin>(sqlCommand, new { Email = email });

                if (admin != null)
                {
                    // Generate a new password
                    string newPassword = Guid.NewGuid().ToString().Substring(0, 8);
                    // You may want to send this password to the user via email or some other method

                    // Encrypt the new password
                    string salt = Guid.NewGuid().ToString();
                    string hashedPassword = GenerateSHA256Hash(newPassword, salt);

                    // Update the password in the database
                    sqlCommand = "UPDATE Admin SET PasswordHash = @PasswordHash, SaltHash = @SaltHash WHERE Id = @Id";
                    conn.Execute(sqlCommand, new { PasswordHash = hashedPassword, SaltHash = salt, Id = admin.Id });

                    // Password reset successful
                    return Ok();
                }
            }
            // Invalid email address
            return NotFound();
        }
    }
}
