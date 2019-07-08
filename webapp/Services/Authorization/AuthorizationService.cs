using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapp.Models.Settings.Authorization;

namespace webapp.Services.Authorization
{
    public class AuthorizationService
    {

        public AuthorizationService()
        {

        }

        public AuthorizationContext GetAuthorizationContext()
        {
            UserContext userContext = new UserContext("1", null, UserRole.GUEST);
            return new AuthorizationContext(false, null, userContext);
        }

    }
}
