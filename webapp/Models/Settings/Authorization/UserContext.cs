using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapp.Models.Settings.Authorization
{
    public class UserContext
    {
        public string Id { get; }
        public string Email { get; }
        public UserRole Role { get; }

        public UserContext(string id, string email, UserRole role)
        {
            Id = id;
            Email = email;
            Role = role;
        }
    }
}
