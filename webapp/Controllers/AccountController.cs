using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using webapp.Models.Settings.Authorization;

namespace webapp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            if(!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }

            if (username == "Admin" && password == "password")
            {
                var identity = new ClaimsIdentity(new[]
                {
                   new Claim(ClaimTypes.Name, username),
                   new Claim(ClaimTypes.Role, UserRole.ADMIN.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                // SignInAsync creates an encrypted cookie (https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.2)
                // When using multiple
                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

    }
}