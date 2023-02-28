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
        public async Task<APIResponse<GetAllStudentDetails>> GetAllStudentDetails(int pageNumber, int pageSize, string sessionClassId)
        {
            var res = new APIResponse<GetAllStudentDetails>();
            try
            {
                var clientId = accessor.HttpContext.Items["smsClientId"].ToString();
                res = await webRequest.GetAsync<APIResponse<GetAllStudentDetails>>($"{fwsOptions.FwsBaseUrl}{FwsRoutes.classStudentsSelect}PageNumber={pageNumber}&PageSize={pageSize}&sessionClassId={sessionClassId}&clientId={clientId}");
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }
        public async Task<APIResponse<List<StudentData>>> GetAllClassStudentDetails(string sessionClassId)
        {
            var res = new APIResponse<List<StudentData>>();
            try
            {
                var clientId = accessor.HttpContext.Items["smsClientId"].ToString();
                res = await webRequest.GetAsync<APIResponse<List<StudentData>>>($"{fwsOptions.FwsBaseUrl}{FwsRoutes.allClassStudentsSelect}sessionClassId={sessionClassId}&clientId={clientId}");
                res.IsSuccessful = true;
                return res;
            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                throw ex;
            }
        }


        public async Task<APIResponse<GetStudentDetails>> GetStudentDetails(string studentRegNo)
        {
            var res = new APIResponse<GetStudentDetails>();
            try
            {
                var clientId = accessor.HttpContext.Items["smsClientId"].ToString();
                res = await webRequest.GetAsync<APIResponse<GetStudentDetails>>($"{fwsOptions.FwsBaseUrl}{FwsRoutes.studentByRegNoSelect}{studentRegNo}&clientId={clientId}");
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
