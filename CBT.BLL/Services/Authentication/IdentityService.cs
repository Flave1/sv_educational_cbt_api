using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Authentication
{
    public class IdentityService : IIdentityService
    {
        private readonly IWebRequest _webRequest;
        private readonly FwsConfigSettings _fwsOptions;
        public IdentityService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions)
        {
            _webRequest = webRequest;
            _fwsOptions = fwsOptions.Value;
        }
        public async Task<LoginSuccessResponse> LoginAsync(LoginCommand user)
        {
            try
            {
                var result = await _webRequest.PostAsync<LoginSuccessResponse, LoginCommand>($"{_fwsOptions.FwsBaseUrl}{FwsRoutes.login}", user);

                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
