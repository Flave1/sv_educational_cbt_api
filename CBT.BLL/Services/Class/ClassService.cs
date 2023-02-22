using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Class;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using CBT.Contracts.Subject;
using CBT.DAL;
using CBT.DAL.Models.Examinations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IWebRequest webRequest;
        private readonly IHttpContextAccessor accessor;
        private readonly FwsConfigSettings fwsOptions;
        public ClassService(IWebRequest webRequest, IOptions<FwsConfigSettings> fwsOptions, IHttpContextAccessor accessor)
        {
            this.webRequest = webRequest;
            this.accessor = accessor;
            this.fwsOptions = fwsOptions.Value;
        }

        public async Task<APIResponse<List<SelectActiveClasses>>> GetActiveClasses()
        {
            var res = new APIResponse<List<SelectActiveClasses>>();
            try
            {
                var clientId = accessor.HttpContext.Items["smsClientId"].ToString();
                var productBaseurlSuffix = accessor.HttpContext.Items["productBaseurlSuffix"].ToString();
                res = await webRequest.GetAsync<APIResponse<List<SelectActiveClasses>>>($"{fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.classSelect}{clientId}");
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }
        public async Task<APIResponse<SelectActiveClasses>> GetActiveClassByRegNo(string registrationNo, string productBaseurlSuffix)
        {
            var res = new APIResponse<SelectActiveClasses>();
            try
            {
                var clientId = accessor.HttpContext.Items["smsClientId"].ToString();
                res = await webRequest.GetAsync<APIResponse<SelectActiveClasses>>($"{fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.classByRegNoSelect}{registrationNo}&clientId={clientId}");
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
