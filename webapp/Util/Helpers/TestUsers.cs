using IdentityModel;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace webapp.Util.Helpers
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "90987897", Username = "admin", Password = "test",
            Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Test Admin")
                }
            }
        };

    }
}
