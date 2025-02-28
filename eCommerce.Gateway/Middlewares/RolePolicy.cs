using System.Diagnostics;
using System.Security.Claims;
using eCommerce.UserService.Protos.V1;
using Microsoft.AspNetCore.Authorization;

namespace eCommerce.Gateway.Middleware
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }

        public RoleRequirement(string requiredRole)
        {
            RequiredRole = requiredRole;
        }
    }

    public class RoleBasedHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly AuthService.AuthServiceClient _authServiceClient;

        public RoleBasedHandler(AuthService.AuthServiceClient authServiceClient)
        {
            _authServiceClient = authServiceClient;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            var userId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            Debug.WriteLine("HandleRequirementAsync");

            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            var response = await _authServiceClient.CheckUserRoleAsync(new RoleRequest
            {
                Id = userId,
                Role = requirement.RequiredRole
            });

            if (response.HasRole)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
