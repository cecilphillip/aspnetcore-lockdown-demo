
namespace DayCare.Web.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Models;
    using Requirements;

    [Authorize(Policy = Constants.StaffPolicyName)]
    public class StaffController : Controller
    {
        private readonly IDayCareService _dayCareService;
        private readonly IAuthorizationService _authorizationService;

        public StaffController(IDayCareService dayCareService, IAuthorizationService authorizationService)
        {
            _dayCareService = dayCareService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = User.Identities.SingleOrDefault(i => i.AuthenticationType == "Local");
            var idClaim = id?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                return Challenge(Constants.AppCookieMiddlewareScheme);
            }
            var staffMember = await _dayCareService.GetStaffMemberAsync(int.Parse(idClaim.Value));
            IEnumerable<Child> allTheChildren = await _dayCareService.GetChildrenAsync();
            return View(new StaffIndexViewModel
            {
                StaffMember = staffMember,
                Children = allTheChildren
            });
        }

        [HttpGet]
        public async Task<IActionResult> TimeLine(int id) => View(await _dayCareService.GetChildAsync(id));

        [HttpGet]
        public async Task<IActionResult> AddNote(int childId)
        {
            var child = await _dayCareService.GetChildAsync(childId);

            if (!await _authorizationService.AuthorizeAsync(User, child, new CanAddNoteRequirement()))
            {
                return Challenge();
            }
           
            return View(new NoteModelViewModel
            {
                ChildId = childId,
                ChildName = child.FirstName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(NoteModelViewModel model)
        {
            if (!await _dayCareService.ChildExistsAsync(model.ChildId))
            {
                ModelState.AddModelError("", "This note cannot be added. Please contact support.");
                return View(model);
            }
            await _dayCareService.AddNoteForChildAsync(new ChildActivity
            {
                ChildId = model.ChildId,
                Title = model.Title,
                Notes = model.Notes
            });

            return RedirectToAction("Timeline", "Staff", new { id = model.ChildId });
        }
    }
}
