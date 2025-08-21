using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ReichertsMeatDistributing.Server.Services
{
    public class AdminAuthorizationService
    {
        private readonly List<string> _authorizedEmails;

        public AdminAuthorizationService(IConfiguration configuration)
        {
            var adminEmails = configuration.GetSection("AdminEmails").Get<List<string>>();
            _authorizedEmails = adminEmails ?? new List<string>
            {
                "jordan.martin.it@gmail.com",
                "tami.r.bickel@gmail.com"
            };
        }

        public bool IsAuthorizedEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && _authorizedEmails.Contains(email);
        }

        public List<string> GetAuthorizedEmails()
        {
            return _authorizedEmails.ToList();
        }

        public bool IsUserAdmin(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            return !string.IsNullOrEmpty(email) && IsAuthorizedEmail(email);
        }
    }
}
