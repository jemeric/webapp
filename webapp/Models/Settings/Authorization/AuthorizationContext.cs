using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Authorization
{
    public class AuthorizationContext
    {
        public bool Authenticated { get; }
        public UserContext User { get; }
        public AuthorizationContext(bool authenticated, UserContext user)
        {
            Authenticated = authenticated;
            User = user;
        }

    }
}
