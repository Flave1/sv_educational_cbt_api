using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Class;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using CBT.Contracts.Subject;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Class
{
    public class ClassService : IClassService
    {
        private readonly IWebRequest _webRequest;
        private readonly FwsConfigSettings _fwsOptions;
        public ClassService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions)
        {
            _webRequest = webRequest;
            _fwsOptions = fwsOptions.Value;
        }
        public async Task<APIResponse<List<SelectActiveClasses>>> GetActiveClasses(string productBaseurlSuffix)
        {
            var res = new APIResponse<List<SelectActiveClasses>>();
            try
            {
                res = await _webRequest.GetAsync<APIResponse<List<SelectActiveClasses>>>($"{_fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.classSelect}");
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
