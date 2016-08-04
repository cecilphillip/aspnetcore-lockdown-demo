﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using DayCare.Web.Services;

namespace DayCare.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [AllowAnonymous]
    public class SecurityController : Controller
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
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

            var guardian = await _securityService.ValidateGuardianCredentialsAsync(loginViewModel.Email, loginViewModel.Password);
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
                await this.HttpContext.Authentication.SignInAsync(Constants.AppCookieMiddlewareScheme, new ClaimsPrincipal(identity));
                return new LocalRedirectResult(returnUrl);
            }
            ModelState.AddModelError("", "Login Failed"); //TODO: what does the view look like now
            return View();
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