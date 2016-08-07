

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DayCare.Web.Models;
using DayCare.Web.Services;

namespace DayCare.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = Constants.GuardianPolicyName)]
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
            var idClaim = User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var g = await _dayCareService.GetGuardianAsync(int.Parse(idClaim.Value));
            return View(g);
        }

        public IActionResult Second()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TimeLine(int id)
        {
           var child = await _dayCareService.GetChildAsync(id);
            return View(child);
        }
    }
}
