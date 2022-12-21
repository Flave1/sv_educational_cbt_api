using CBT.Contracts;
using CBT.Contracts.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Subject
{
    public interface ISubjectService
    {
        Task<APIResponse<List<SelectSubjects>>> GetActiveSubjects();
        Task<APIResponse<List<SelectSessionClassSubjects>>> GetActiveSessionClassSubjects(string sessionClassId);
    }
}
