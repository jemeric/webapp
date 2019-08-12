using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace webapp.Services.Authorization
{
    public class AuthorizationService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorizationService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public AuthorizationContext GetAuthorizationContext()
        {
            HttpContext context = this.httpContextAccessor.HttpContext;

            var user = context.User;
            bool isAuthenticated = user.Identity.IsAuthenticated;

            var role = user.FindFirst(ClaimTypes.Role)?.Value ?? UserRole.Guest;

            UserContext userContext = new UserContext(user.Identity.Name, null, role);
            return new AuthorizationContext(isAuthenticated, userContext);
        }

    }
}
