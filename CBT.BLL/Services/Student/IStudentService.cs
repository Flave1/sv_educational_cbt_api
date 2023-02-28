using CBT.Contracts.Session;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Student;

namespace CBT.BLL.Services.Student
{
    public interface IStudentService
    {
        Task<APIResponse<GetStudentDetails>> GetStudentDetails(string studentRegNo);
        Task<APIResponse<GetAllStudentDetails>> GetAllStudentDetails(int pageNumber, int pageSize, string sessionClassId);
        Task<APIResponse<List<StudentData>>> GetAllClassStudentDetails(string sessionClassId);
    }
}
