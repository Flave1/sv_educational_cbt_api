using CBT.BLL.Constants;
using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBT.BLL.Services.Authentication
{
    public class IdentityService : IIdentityService
    {
        private readonly IWebRequest webRequest;
        private readonly IConfiguration config;
        private readonly FwsConfigSettings fwsOptions;
        public IdentityService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions, IConfiguration config)
        {
            this.webRequest = webRequest;
            this.config = config;
            this.fwsOptions = fwsOptions.Value;
        }
        public async Task<APIResponse<LoginDetails>> SMPLoginAsync(LoginCommandByHash user)
        {
            var res = new APIResponse<LoginDetails>();
            try
            {
                var result = await webRequest.PostAsync<LoginSuccessResponse, LoginCommandByHash>($"{fwsOptions.FwsBaseUrl}{FwsRoutes.loginByHash}", user);
                if (result.Result.AuthResult.Token != null)
                {
                    res.Result = await ReadAndGenerateNewTokenAsync(result.Result.AuthResult.Token);
                    res.Result.ClientUrl = config.GetValue<string>("ClientUrl");
                    res.IsSuccessful = true;
                    return res;
                }
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = result.Message.FriendlyMessage?.ToString();
                res.Message.TechnicalMessage = result.Message.TechnicalMessage?.ToString();
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<APIResponse<LoginDetails>> LoginAsync(LoginCommand user)
        {
            var res = new APIResponse<LoginDetails>();
            try
            {
                var result = await webRequest.PostAsync<LoginSuccessResponse, LoginCommand>($"{fwsOptions.FwsBaseUrl}{FwsRoutes.login}", user);
                if (result.Result.AuthResult.Token != null)
                {
                    res.Result = await ReadAndGenerateNewTokenAsync(result.Result.AuthResult.Token);
                    res.IsSuccessful = true;
                    return res;
                }
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = result.Message.FriendlyMessage?.ToString();
                res.Message.TechnicalMessage = result.Message.TechnicalMessage?.ToString();
                return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        private async Task<LoginDetails> ReadAndGenerateNewTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokena = handler.ReadJwtToken(token);

            var userId = tokena.Claims.FirstOrDefault(x => x.Type == "userId").Value;
            var smsClientId = tokena.Claims.FirstOrDefault(x => x.Type == "smsClientId").Value;
            var productBaseurlSuffix = tokena.Claims.FirstOrDefault(x => x.Type == "productBaseurlSuffix").Value;
            var email = tokena.Claims.FirstOrDefault(x => x.Type == "email").Value;

            int userType;

            if (string.IsNullOrEmpty(smsClientId))
            {
                userType = (int)UserType.NonSMSUser;
            }
            else
            {
                userType = (int)UserType.SMSUser;
            }

            var userDetails = new UserDetails
            {
                Email = email,
                UserId = userId,
                UserType = userType
            };

            var claims = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, email),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Email, email),
               new Claim("userId", userId),
               new Claim("smsClientId", smsClientId),
               new Claim("productBaseurlSuffix", productBaseurlSuffix),
               new Claim("userType", userType.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Jwt:Key").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var newToken = tokenHandler.CreateToken(tokenDescriptor);

            var authDetails = new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(newToken)
            };

            return new LoginDetails { AuthResult = authDetails, UserDetails = userDetails };
        }
    }
}
