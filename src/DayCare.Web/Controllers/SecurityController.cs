namespace DayCare.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [AllowAnonymous]
    public class SecurityController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel loginViewModel)
        {

        }

        //http://stackoverflow.com/questions/3521290/logout-get-or-post

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.AppCookieMiddlewareScheme);
            return Redirect("/");
        }

        public IActionResult Denied()
        {
            return View();
        }
    }
}
