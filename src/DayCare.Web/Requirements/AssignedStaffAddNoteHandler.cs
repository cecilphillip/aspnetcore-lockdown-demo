namespace DayCare.Web.Requirements
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Models;
    using Services;

    public class AssignedStaffAddNoteHandler : AuthorizationHandler<CanAddNoteRequirement, Child>
    {
        private readonly IDayCareService _dayCareService;

        public AssignedStaffAddNoteHandler(IDayCareService dayCareService)
        {
            _dayCareService = dayCareService;
        }      

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAddNoteRequirement requirement, Child resource)
        {          
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Staff"))
            {
                var idClaim = context.User.Claims.SingleOrDefault(c => c.Issuer == "Local" && c.Type == ClaimTypes.NameIdentifier);
                if (idClaim == null)
                {
                    context.Fail();
                    return;
                }

                var staffMember = await _dayCareService.GetStaffMemberAsync(int.Parse(idClaim.Value));
                if (staffMember == null)
                {
                    context.Fail();
                    return;
                }

                var isAssigned = staffMember.ChildAssignments.Any(ca => ca.ChildId == resource.Id);
                if (isAssigned)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}