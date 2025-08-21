using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ReichertsMeatDistributing.Server.Services
{
    public class AdminRequirement : IAuthorizationRequirement
    {
        public AdminRequirement() { }
    }

    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly AdminAuthorizationService _adminAuth;

        public AdminAuthorizationHandler(AdminAuthorizationService adminAuth)
        {
            _adminAuth = adminAuth;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                
                if (!string.IsNullOrEmpty(email) && _adminAuth.IsAuthorizedEmail(email))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
