using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Authentication
{
    public class LoginDetails
    {
        public AuthenticationResult AuthResult { get; set; }
        public UserDetails UserDetails { get; set; }
    }
    public class UserDetails
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
    }
}
