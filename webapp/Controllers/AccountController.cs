using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using webapp.Models.Settings.Authorization;
using webapp.Services.Assets;
using webapp.Util.Dto.Views;

namespace webapp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAssetsService assetsService;

        public AccountController(IAssetsService assetsService)
        {
            this.assetsService = assetsService;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            string currentVersion = await assetsService.GetCurrentVersion();
            return View(new LoginData(currentVersion));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            string currentVersion = await assetsService.GetCurrentVersion();
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }

            if (username == "Admin" && password == "password")
            {
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, username),
                   new Claim(ClaimTypes.Role, UserRole.Admin)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                // SignInAsync creates an encrypted cookie (https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2)
                // When using multiple
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginData(currentVersion));
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}