using CBT.Contracts.Subject;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Session;
using CBT.Contracts.Routes;
using CBT.BLL.Services.WebRequests;
using CBT.Contracts.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CBT.BLL.Services.Session
{
    public class SessionService : ISessionService
    {
        private readonly IWebRequest _webRequest;
        private readonly IHttpContextAccessor _accessor;
        private readonly FwsConfigSettings _fwsOptions;
        public SessionService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions, IHttpContextAccessor accessor)
        {
            _webRequest = webRequest;
            _accessor = accessor;
            _fwsOptions = fwsOptions.Value;
        }
        public async Task<APIResponse<SelectActiveSession>> GetActiveSession(int examScore, bool asExamScore, bool asAssessmentScore)
        {
            var res = new APIResponse<SelectActiveSession>();
            try
            {
                var clientId = _accessor.HttpContext.Items["smsClientId"].ToString();
                res = await _webRequest.GetAsync<APIResponse<SelectActiveSession>>($"{_fwsOptions.FwsBaseUrl}{FwsRoutes.activeSessionSelect}{examScore}&asExamScore={asExamScore}&asAssessmentScore={asAssessmentScore}&clientId={clientId}");
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }
    }
}
