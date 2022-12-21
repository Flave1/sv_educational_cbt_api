using CBT.Contracts.Session;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Result;
using CBT.BLL.Wrappers;
using CBT.BLL.Filters;

namespace CBT.BLL.Services.Result
{
    public interface IResultService
    {
        Task<APIResponse<SelectResult>> GetCandidateResult();
        Task<APIResponse<PagedResponse<List<SelectAllCandidateResult>>>> GetAllCandidateResult(PaginationFilter filter, string examinationId);
    }
}
