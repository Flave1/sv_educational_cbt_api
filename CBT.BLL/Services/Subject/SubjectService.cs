using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using CBT.Contracts.Subject;
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
        private readonly FwsConfigSettings _fwsOptions;
        public SubjectService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions)
        {
            _webRequest = webRequest;
            _fwsOptions = fwsOptions.Value;
        }
        public async Task<APIResponse<List<SelectSubjects>>> GetActiveSubjects()
        {
            var res = new APIResponse<List<SelectSubjects>>();
            try
            {
               res = await _webRequest.GetAsync<APIResponse<List<SelectSubjects>>>($"{_fwsOptions.FwsBaseUrl}smp/development/subject/api/v1/getall/active-subject");

               return res;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
