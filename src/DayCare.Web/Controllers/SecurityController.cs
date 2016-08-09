namespace DayCare.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using AspNet.Security.OAuth.GitHub;
    using Services;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [AllowAnonymous]
    public class SecurityController : Controller
    {
        private readonly IDayCareService _dayCareService;

        public SecurityController(IDayCareService dayCareService)
        {
            _dayCareService = dayCareService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(loginViewModel);

            switch (loginViewModel.LoginType)
            {
                case LoginType.Guardian:
                    return await SignInGuardian(loginViewModel, returnUrl);
                case LoginType.Staff:
                    return await SignInStaff(loginViewModel, returnUrl);
                default:
                    break;
            }
            ModelState.AddModelError("", "Invalid Login Type");
            return View("Login");
        }

        private async Task<IActionResult> SignInStaff(LoginViewModel loginViewModel, string returnUrl)
        {
            var staff = await _dayCareService.ValidateStaffCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (staff != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString(),ClaimValueTypes.Integer, "Local"),
                    new Claim(ClaimTypes.Name,$"{staff.FirstName} {staff.LastName}"),

                    staff.FirstName == "admin"?  new Claim(ClaimTypes.Role, "Admin"): new Claim(ClaimTypes.Role, "Staff"),

                    new Claim(ClaimTypes.Email, loginViewModel.Email)
                };

                var identity = new ClaimsIdentity(claims, "Local");

                var props = new AuthenticationProperties
                {
                    IsPersistent = loginViewModel.RememberMe,
                };

                await HttpContext.Authentication.SignInAsync(Constants.AppCookieMiddlewareScheme, new ClaimsPrincipal(identity), props);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Staff");
                }

                return new LocalRedirectResult(returnUrl);
            }
            ModelState.AddModelError("", "Login Failed");
            return View("Login");
        }

        private async Task<IActionResult> SignInGuardian(LoginViewModel loginViewModel, string returnUrl)
        {
            var guardian = await _dayCareService.ValidateGuardianCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (guardian != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, guardian.Id.ToString()),
                    new Claim(ClaimTypes.Name,$"{guardian.FirstName} {guardian.LastName}"),
                    new Claim(ClaimTypes.Role, "Guardian"),
                    new Claim(ClaimTypes.Email, loginViewModel.Email)
                };

                var identity = new ClaimsIdentity(claims, "Local");

                var props = new AuthenticationProperties
                {
                    IsPersistent = loginViewModel.RememberMe,
                };

                await this.HttpContext.Authentication.SignInAsync(Constants.AppCookieMiddlewareScheme, new ClaimsPrincipal(identity), props);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index", "Home");
                }

                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError("", "Login Failed");
            return View("Login");
        }

        [HttpPost]
        public IActionResult GitHubSignin()
        {
            var callbackUrl = this.Url.Action(nameof(GitHubCallback), "Security");
            return Challenge(new AuthenticationProperties { RedirectUri = callbackUrl, }, GitHubAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GitHubCallback()
        {
            var temp = await HttpContext.Authentication.AuthenticateAsync(Constants.TempCookieMiddlewareScheme);
            if (temp == null)
            {
                // oh oh...... :(
                return RedirectToAction(nameof(Login));
            }

            // Login Github user as Admin
            var staffAdmin = await _dayCareService.GetStaffMemberAsync("admin");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, staffAdmin.Id.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.Name, temp.Claims.SingleOrDefault(s=> s.Type == "urn:github:name")?.Value)
            };

            var identity = new ClaimsIdentity(claims, "Local");

            await HttpContext.Authentication.SignInAsync(Constants.AppCookieMiddlewareScheme, new ClaimsPrincipal(identity));
            await HttpContext.Authentication.SignOutAsync(Constants.TempCookieMiddlewareScheme);

            return RedirectToAction(nameof(StaffController.Index), "Staff");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync(Constants.AppCookieMiddlewareScheme);
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult Denied()
        {
            return View();
        }
    }
}
