
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

    [Authorize(Policy = Constants.StaffPolicyName)]
    public class StaffController : Controller
    {
        private readonly IDayCareService _dayCareService;

        public StaffController(IDayCareService dayCareService)
        {
            _dayCareService = dayCareService;
        }

        public async Task<IActionResult> Index()
        {
            var idClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var staffMember = await _dayCareService.GetStaffMemberAsync(int.Parse(idClaim.Value));
            IEnumerable<Child> allTheChildren = await _dayCareService.GetChildrenAsync();
            return View(new StaffIndexViewModel
            {
                StaffMember = staffMember,
                Children = allTheChildren
            });
        }

        [HttpGet]
        public async Task<IActionResult> TimeLine(int id)
        {
            var child = await _dayCareService.GetChildAsync(id);
            return View(child);
        }

        [HttpGet]
        public async Task<IActionResult> AddNote(int childId)
        {
            var child = await _dayCareService.GetChildAsync(childId);
            //TODO: handle not found
            return View(new NoteModelViewModel
            {
                ChildId = childId,
                ChildName = child.FirstName
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddNote(NoteModelViewModel model)
        {
            if (!await _dayCareService.ChildExistsAsync(model.ChildId))
            {
                //TODO Handle child not found
            }
            await _dayCareService.AddNoteForChildAsync(new ChildActivity
            {
                ChildId = model.ChildId,
                Title = model.Title,
                Notes = model.Notes
            });

            return RedirectToAction("Timeline", "Staff", new { id = model.ChildId});
        }
    }
}
