using CBT.Contracts.Subject;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Class;
using CBT.Contracts.Session;

namespace CBT.BLL.Services.Session
{
    public interface ISessionService
    {
        Task<APIResponse<SelectActiveSession>> GetActiveSession(int examScore, bool asExamScore, bool asAssessmentScore);
    }
}
