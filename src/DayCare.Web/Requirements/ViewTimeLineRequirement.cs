namespace DayCare.Web.Requirements
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;

    public class ViewTimeLineRequirement : AuthorizationHandler<ViewTimeLineRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViewTimeLineRequirement requirement)
        {
            var isGuardian = context.User.HasClaim(ClaimTypes.Role, "Guardian");
            if (isGuardian)
            {
                if (!context.User.Identity.Name.StartsWith("Uncle"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
