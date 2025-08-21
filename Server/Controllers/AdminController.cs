using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReichertsMeatDistributing.Server.Data;
using ReichertsMeatDistributing.Shared;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ReichertsMeatDistributing.Server.Services;

namespace ReichertsMeatDistributing.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProductSeederService _productSeeder;
        private readonly AdminAuthorizationService _adminAuth;

        public AdminController(AppDbContext context, ProductSeederService productSeeder, AdminAuthorizationService adminAuth)
        {
            _context = context;
            _productSeeder = productSeeder;
            _adminAuth = adminAuth;
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

        [HttpGet("google-login")]
        public IActionResult GoogleLogin(string returnUrl = "/admin/deals")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = "/admin/deals",
                Items =
                {
                    { ".xsrf", Guid.NewGuid().ToString() }
                }
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            // This endpoint will be called after Google authentication
            // The actual authentication is handled by the middleware
            return Redirect("/admin/deals");
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleSignInCallback()
        {
            Console.WriteLine("=== GOOGLE SIGNIN CALLBACK ===");
            Console.WriteLine($"User.Identity?.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User.Identity?.Name: {User.Identity?.Name}");
            Console.WriteLine($"User.Claims count: {User.Claims.Count()}");
            
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                Console.WriteLine($"Successfully authenticated: {email}");
                
                // Redirect to admin deals page
                return Redirect("/admin/deals");
            }
            else
            {
                Console.WriteLine("Authentication failed in callback");
                return Redirect("/login?error=auth_failed");
            }
        }

        [HttpGet("auth-status")]
        public IActionResult GetAuthStatus()
        {
            Console.WriteLine($"=== AUTH STATUS CHECK ===");
            Console.WriteLine($"User.Identity?.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User.Identity?.Name: {User.Identity?.Name}");
            Console.WriteLine($"User.Identity?.AuthenticationType: {User.Identity?.AuthenticationType}");
            Console.WriteLine($"User.Claims count: {User.Claims.Count()}");
            
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                Console.WriteLine($"AUTH SUCCESS: Email={email}, Name={name}");
                Console.WriteLine($"=== END AUTH STATUS CHECK ===");
                return Ok(new { isAuthenticated = true, email, name });
            }
            
            Console.WriteLine($"AUTH FAILED: User not authenticated");
            Console.WriteLine($"=== END AUTH STATUS CHECK ===");
            return Ok(new { isAuthenticated = false });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Console.WriteLine("=== LOGOUT INITIATED ===");
                Console.WriteLine($"User: {User?.FindFirst(ClaimTypes.Email)?.Value}");
                
                // Clear all authentication schemes
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);
                
                // Clear any additional authentication cookies
                Response.Cookies.Delete(".AspNetCore.Cookies");
                Response.Cookies.Delete(".AspNetCore.Authentication.Google");
                
                Console.WriteLine("=== LOGOUT COMPLETED ===");
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                return Ok(new { message = "Logout completed" });
            }
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

        [HttpGet("product-count")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetProductCount()
        {
            try
            {
                var productCount = await _context.Products.CountAsync();
                return Ok(new { ProductCount = productCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error getting product count", error = ex.Message });
            }
        }

        [HttpPost("reseed-products")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ReseedProducts()
        {
            try
            {
                await _productSeeder.ForceSeedProductsAsync();
                var productCount = await _context.Products.CountAsync();
                return Ok(new { Message = $"Successfully reseeded {productCount} products", ProductCount = productCount });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReseedProducts endpoint: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = "Error reseeding products", error = ex.Message, details = ex.StackTrace });
            }
        }

        [HttpPost("reseed-products-dev")]
        public async Task<IActionResult> ReseedProductsDev()
        {
            try
            {
                await _productSeeder.ForceSeedProductsAsync();
                var productCount = await _context.Products.CountAsync();
                return Ok(new { Message = $"Successfully reseeded {productCount} products", ProductCount = productCount });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error reseeding products", error = ex.Message });
            }
        }

        [HttpGet("test-response")]
        public IActionResult TestResponse()
        {
            return Ok(new { Message = "Test response", ProductCount = 42 });
        }

        [AllowAnonymous]
        [HttpGet("manual-login")]
        public async Task<IActionResult> ManualLogin()
        {
            Console.WriteLine("=== MANUAL LOGIN TEST ===");
            
            // Create claims for your email
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "jordan.martin.it@gmail.com"),
                new Claim(ClaimTypes.Name, "Jordan Martin"),
                new Claim(ClaimTypes.NameIdentifier, "jordan.martin.it@gmail.com")
            };
            
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);
            
            Console.WriteLine("Manual login completed");
            Console.WriteLine("=== END MANUAL LOGIN ===");
            
            // Instead of redirecting, let's return a simple page to test if we stay authenticated
            return Content(@"
                <html>
                <body>
                    <h1>Manual Login Test</h1>
                    <p>If you can see this page, the login worked!</p>
                    <p>Now try going to: <a href='/admin/deals'>Admin Deals</a></p>
                    <p>Or check auth status: <a href='/api/admin/debug-auth'>Debug Auth</a></p>
                    <script>
                        setTimeout(function() {
                            window.location.href = '/admin/deals';
                        }, 5000);
                    </script>
                </body>
                </html>", "text/html");
        }

        [AllowAnonymous]
        [HttpGet("simple-test")]
        public IActionResult SimpleTest()
        {
            return Content(@"
                <html>
                <body>
                    <h1>Simple Test Page</h1>
                    <p>This page should work without authentication.</p>
                    <p>If you can see this, the basic routing is working.</p>
                </body>
                </html>", "text/html");
        }

        [AllowAnonymous]
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            Console.WriteLine("=== DEBUG AUTH ENDPOINT ===");
            Console.WriteLine($"User.Identity?.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User.Identity?.Name: {User.Identity?.Name}");
            Console.WriteLine($"User.Identity?.AuthenticationType: {User.Identity?.AuthenticationType}");
            Console.WriteLine($"User.Claims count: {User.Claims.Count()}");
            
            var claims = new List<object>();
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                claims.Add(new { Type = claim.Type, Value = claim.Value });
            }
            
            var cookies = Request.Cookies.Select(c => new { Name = c.Key, Value = c.Value }).ToList();
            Console.WriteLine($"Cookies count: {cookies.Count}");
            foreach (var cookie in cookies)
            {
                Console.WriteLine($"Cookie: {cookie.Name} = {cookie.Value}");
            }
            
            Console.WriteLine("=== END DEBUG AUTH ===");
            
            return Ok(new { 
                isAuthenticated = User.Identity?.IsAuthenticated ?? false,
                userName = User.Identity?.Name,
                authType = User.Identity?.AuthenticationType,
                claimsCount = User.Claims.Count(),
                claims = claims,
                cookies = cookies
            });
        }

        [HttpGet("repository-count")]
        public IActionResult GetRepositoryCount()
        {
            try
            {
                var repositoryCount = _productSeeder.GetRepositoryProductCount();
                var databaseCount = _context.Products.Count();
                Console.WriteLine($"Repository count: {repositoryCount}, Database count: {databaseCount}");
                return Ok(new { 
                    RepositoryCount = repositoryCount, 
                    DatabaseCount = databaseCount,
                    Difference = repositoryCount - databaseCount
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRepositoryCount: {ex.Message}");
                return BadRequest(new { message = "Error getting repository count", error = ex.Message });
            }
        }

        [HttpGet("category-distribution")]
        public async Task<IActionResult> GetCategoryDistribution()
        {
            try
            {
                var distribution = await _productSeeder.GetDatabaseCategoryDistributionAsync();
                return Ok(new { CategoryDistribution = distribution });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to get category distribution", details = ex.Message });
            }
        }

            [HttpGet("access-denied")]
    public IActionResult AccessDenied()
    {
        return Unauthorized(new { message = "Access denied. You are not authorized to access this resource." });
    }

    [HttpGet("admin-config")]
    public IActionResult GetAdminConfig()
    {
        var authorizedEmails = _adminAuth.GetAuthorizedEmails();
        return Ok(new { 
            authorizedEmails,
            totalCount = authorizedEmails.Count,
            message = "Admin configuration retrieved successfully"
        });
    }

        [HttpGet("validate-admin")]
        public IActionResult ValidateAdmin()
        {
            Console.WriteLine("=== ADMIN VALIDATION CHECK ===");
            Console.WriteLine($"Is Authenticated: {User.Identity?.IsAuthenticated}");
            
            if (User.Identity?.IsAuthenticated == true)
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var isAdmin = _adminAuth.IsUserAdmin(User);
                
                Console.WriteLine($"User Email: {email}");
                Console.WriteLine($"Is Admin: {isAdmin}");
                Console.WriteLine($"Authorized Emails: {string.Join(", ", _adminAuth.GetAuthorizedEmails())}");

                if (isAdmin)
                {
                    Console.WriteLine("=== ADMIN VALIDATION SUCCESS ===");
                    return Ok(new { isAdmin = true, email, message = "Access granted" });
                }
                else
                {
                    Console.WriteLine("=== ADMIN VALIDATION FAILED ===");
                    return Unauthorized(new { isAdmin = false, email, message = "Email not in admin list" });
                }
            }
            
            Console.WriteLine("=== USER NOT AUTHENTICATED ===");
            return Unauthorized(new { isAdmin = false, message = "Not authenticated" });
        }

        [HttpPost("force-logout")]
        public async Task<IActionResult> ForceLogout()
        {
            Console.WriteLine("=== FORCE LOGOUT INITIATED ===");
            
            try
            {
                // Sign out all schemes
                await HttpContext.SignOutAsync();
                
                // Clear specific cookies
                var cookieNames = new[] { 
                    ".AspNetCore.Cookies", 
                    ".AspNetCore.Authentication.Google",
                    ".AspNetCore.Antiforgery",
                    "__Host-spa"
                };
                
                foreach (var cookieName in cookieNames)
                {
                    Response.Cookies.Delete(cookieName);
                    Console.WriteLine($"Deleted cookie: {cookieName}");
                }
                
                Console.WriteLine("=== FORCE LOGOUT COMPLETED ===");
                return Ok(new { message = "Force logout completed successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Force logout error: {ex.Message}");
                return Ok(new { message = "Force logout completed with errors" });
            }
        }
    }
}

