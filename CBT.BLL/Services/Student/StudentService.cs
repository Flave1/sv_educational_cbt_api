using CBT.Contracts.Session;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Student;
using CBT.Contracts.Routes;
using CBT.BLL.Services.WebRequests;
using CBT.Contracts.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Drawing.Printing;

namespace CBT.BLL.Services.Student
{
    public class StudentService : IStudentService
    {
        private readonly FwsConfigSettings fwsOptions;
        private readonly IWebRequest webRequest;
        private readonly IHttpContextAccessor accessor;

        public StudentService(IWebRequest webRequest, IHttpContextAccessor accessor, IOptions<FwsConfigSettings> fwsOptions)
        {
            this.fwsOptions = fwsOptions.Value;
            this.webRequest = webRequest;
            this.accessor = accessor;
        }
        public async Task<APIResponse<GetAllStudentDetails>> GetAllStudentDetails(int pageNumber, int pageSize, string sessionClassId, string productBaseurlSuffix)
        {
            var res = new APIResponse<GetAllStudentDetails>();
            try
            {
                res = await webRequest.GetAsync<APIResponse<GetAllStudentDetails>>($"{fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.classStudentsSelect}PageNumber={pageNumber}&PageSize={pageSize}&sessionClassId={sessionClassId}");
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }
        public async Task<APIResponse<GetAllStudentDetails>> GetAllClassStudentDetails(string sessionClassId, string productBaseurlSuffix)
        {
            var res = new APIResponse<GetAllStudentDetails>();
            try
            {
                res = await webRequest.GetAsync<APIResponse<GetAllStudentDetails>>($"{fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.allClassStudentsSelect}sessionClassId={sessionClassId}");
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }


        public async Task<APIResponse<GetStudentDetails>> GetStudentDetails(string studentRegNo, string productBaseurlSuffix)
        {
            var res = new APIResponse<GetStudentDetails>();
            try
            {
                res = await webRequest.GetAsync<APIResponse<GetStudentDetails>>($"{fwsOptions.FwsBaseUrl}smp/{productBaseurlSuffix}{FwsRoutes.studentByRegNoSelect}{studentRegNo}");
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
