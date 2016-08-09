namespace DayCare.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize (Policy = Constants.GuardianPolicyName)]
    public class HomeController : Controller
    {
        private readonly IDayCareService _dayCareService;

        public HomeController(IDayCareService dayCareService)
        {
            _dayCareService = dayCareService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var id = User.Identities.SingleOrDefault(i => i.AuthenticationType == "Local");
            var idClaim = id?.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            var g = await _dayCareService.GetGuardianAsync(int.Parse(idClaim.Value));
            return View(g);
        }

        public IActionResult Second() => View();
        

        [HttpGet]
        public async Task<IActionResult> TimeLine(int id) => View(await _dayCareService.GetChildAsync(id));        
    }
}
