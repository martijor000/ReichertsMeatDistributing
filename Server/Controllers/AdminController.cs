using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ReichertsMeatDistributing.Server.Model;
using ReichertsMeatDistributing.Shared;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
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
    }
}
