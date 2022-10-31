using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Authentication
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
    public class LoginCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class Message
    {
        public object FriendlyMessage { get; set; }
        public object TechnicalMessage { get; set; }
    }
    public class Result
    {
        public AuthenticationResult AuthResult { get; set; }
    }
    public class LoginSuccessResponse
    {
        public Result Result { get; set; }
        public string Status { get; set; }
        public Message Message { get; set; }
    }
}
