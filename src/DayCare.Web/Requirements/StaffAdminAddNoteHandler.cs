namespace DayCare.Web.Requirements
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Models;

    public class StaffAdminAddNoteHandler : AuthorizationHandler<CanAddNoteRequirement, Child>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAddNoteRequirement requirement, Child resource)
        {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}