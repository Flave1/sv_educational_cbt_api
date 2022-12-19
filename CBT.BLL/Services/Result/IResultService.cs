using CBT.Contracts.Session;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Result;

namespace CBT.BLL.Services.Result
{
    public interface IResultService
    {
        Task<APIResponse<SelectResult>> GetCandidateResult();
        Task<APIResponse<List<SelectAllCandidateResult>>> GetAllCandidateResult(string examinationId);
    }
}
