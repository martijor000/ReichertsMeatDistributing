using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using ReichertsMeatDistributing.Server.Services;
using System.Security.Claims;

namespace ReichertsMeatDistributing.Server.Middleware
{
    public class AdminValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminValidationMiddleware> _logger;

        public AdminValidationMiddleware(RequestDelegate next, ILogger<AdminValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, AdminAuthorizationService adminAuth)
        {
            // Only check admin paths
            if (context.Request.Path.StartsWithSegments("/api/admin") && 
                !context.Request.Path.StartsWithSegments("/api/admin/google-login") &&
                !context.Request.Path.StartsWithSegments("/api/admin/access-denied") &&
                !context.Request.Path.StartsWithSegments("/api/admin/validate-admin"))
            {
                _logger.LogInformation($"=== ADMIN MIDDLEWARE: {context.Request.Path} ===");
                
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                    var name = context.User.FindFirst(ClaimTypes.Name)?.Value;
                    _logger.LogInformation($"Authenticated user: {email} ({name})");
                    
                    if (!adminAuth.IsUserAdmin(context.User))
                    {
                        _logger.LogWarning($"UNAUTHORIZED ACCESS: Email '{email}' not in admin list");
                        _logger.LogWarning($"Authorized emails: {string.Join(", ", adminAuth.GetAuthorizedEmails())}");
                        
                        // Sign out the unauthorized user
                        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized: You are not authorized to access admin functionality.");
                        return;
                    }
                    
                    _logger.LogInformation($"AUTHORIZED ACCESS: {email} granted access to {context.Request.Path}");
                }
                else
                {
                    _logger.LogWarning("UNAUTHENTICATED ACCESS: No valid authentication token");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Authentication required.");
                    return;
                }
                
                _logger.LogInformation($"=== END ADMIN MIDDLEWARE ===");
            }

            await _next(context);
        }
    }
}
