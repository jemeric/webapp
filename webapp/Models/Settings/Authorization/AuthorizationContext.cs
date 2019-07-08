using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Authorization
{
    public class AuthorizationContext
    {
        public bool Authenticated { get; }
        public string AccessToken { get; }
        public UserContext User { get; }
        public AuthorizationContext(bool authenticated, string accessToken, UserContext user)
        {
            Authenticated = authenticated;
            AccessToken = accessToken;
            User = user;
        }

    }
}
