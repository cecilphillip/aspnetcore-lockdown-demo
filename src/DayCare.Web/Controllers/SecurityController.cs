﻿using System.Collections.Generic;
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
            var guardian = await _dayCareService.ValidateStaffCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
            if (guardian != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, guardian.Id.ToString(),ClaimValueTypes.Integer, "Local"),
                    new Claim(ClaimTypes.Name,$"{guardian.FirstName} {guardian.LastName}"),

                    guardian.FirstName == "admin"?  new Claim(ClaimTypes.Role, "Admin"): new Claim(ClaimTypes.Role, "Staff"),

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

                return new LocalRedirectResult(returnUrl);
            }
            ModelState.AddModelError("", "Login Failed");
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
