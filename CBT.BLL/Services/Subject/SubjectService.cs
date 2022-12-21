using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using CBT.Contracts.Subject;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Subject
{
    public class SubjectService : ISubjectService
    {
        private readonly IWebRequest _webRequest;
        private readonly IHttpContextAccessor _accessor;
        private readonly FwsConfigSettings _fwsOptions;
        public SubjectService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions, IHttpContextAccessor accessor)
        {
            _webRequest = webRequest;
            _accessor = accessor;
            _fwsOptions = fwsOptions.Value;
        }

        public async Task<APIResponse<List<SelectSubjects>>> GetActiveSubjects()
        {
            var res = new APIResponse<List<SelectSubjects>>();
            try
            {
                var productBaseurlSuffix = _accessor.HttpContext.Items["productBaseurlSuffix"].ToString();
                res = await _webRequest.GetAsync<APIResponse<List<SelectSubjects>>>($"{_fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.subjectSelect}");
                res.IsSuccessful = true;
                return res;
            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }
        public async Task<APIResponse<List<SelectSessionClassSubjects>>> GetActiveSessionClassSubjects(string sessionClassId)
        {
            var res = new APIResponse<List<SelectSessionClassSubjects>>();
            try
            {
                var productBaseurlSuffix = _accessor.HttpContext.Items["productBaseurlSuffix"].ToString();
                res = await _webRequest.GetAsync<APIResponse<List<SelectSessionClassSubjects>>>($"{_fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.sessionSubjectSelect}{sessionClassId}");
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
