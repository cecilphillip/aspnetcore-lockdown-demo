using System;
using System.Collections.Generic;
using System.Security.Claims;
using DayCare.Web.Services;

namespace DayCare.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> LoginGuardian(LoginViewModel loginViewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var guardian = await _dayCareService.ValidateGuardianCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (guardian != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, guardian.Id.ToString()),
                    new Claim(ClaimTypes.Name,$"{guardian.FirstName} {guardian.LastName}"),
                    new Claim(ClaimTypes.Role, "guardian"),
                    new Claim(ClaimTypes.Email, loginViewModel.Email)
                };

                var identity = new ClaimsIdentity(claims, "Local");

                var props = new AuthenticationProperties
                {
                    IsPersistent = loginViewModel.RememberMe,                    
                };

                await this.HttpContext.Authentication.SignInAsync(Constants.AppCookieMiddlewareScheme, new ClaimsPrincipal(identity), props);
                return new LocalRedirectResult(string.IsNullOrEmpty(returnUrl)? "/" : returnUrl);
            }
            ModelState.AddModelError("", "Login Failed"); //TODO: what does the view look like now
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> LoginStaff(LoginViewModel loginViewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var guardian = await _dayCareService.ValidateGuardianCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (guardian != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, guardian.Id.ToString()),
                    new Claim(ClaimTypes.Name,$"{guardian.FirstName} {guardian.LastName}"),
                    new Claim(ClaimTypes.Role, "guardian"),
                    new Claim(ClaimTypes.Email, loginViewModel.Email)
                };

                var identity = new ClaimsIdentity(claims, "Local");

                var props = new AuthenticationProperties
                {
                    IsPersistent = loginViewModel.RememberMe,
                };

                await this.HttpContext.Authentication.SignInAsync(Constants.AppCookieMiddlewareScheme, new ClaimsPrincipal(identity), props);
                return new LocalRedirectResult(returnUrl);
            }
            ModelState.AddModelError("", "Login Failed"); //TODO: what does the view look like now
            return View("Login");
        }

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
