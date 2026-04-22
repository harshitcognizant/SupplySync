using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SupplySync.Security
{
    public class SingleRoleHandler : AuthorizationHandler<SingleRoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SingleRoleRequirement requirement)
        {
            if (!context.User.Identity?.IsAuthenticated ?? false)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var roleClaims = context.User.FindAll(ClaimTypes.Role);
            // Exactly one role claim required
            if (roleClaims != null && roleClaims.Count() == 1)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}