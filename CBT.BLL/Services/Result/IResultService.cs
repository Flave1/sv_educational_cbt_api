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
using CBT.Contracts.CandidateAnswers;

namespace CBT.BLL.Services.Result
{
    public interface IResultService
    {
        Task<APIResponse<SelectResult>> GetCandidateResult();
        Task<APIResponse<PagedResponse<List<SelectAllCandidateResult>>>> GetAllCandidateResult(PaginationFilter filter, string examinationId);
        Task<APIResponse<byte[]>> DownloadCandidateResult(string examinationId);
        Task<APIResponse<PagedResponse<List<SelectCandidateAnswer>>>> GetCandidateAnswers(PaginationFilter filter, string examinationId, string candidateId_regNo);
        Task<APIResponse<List<SelectAllCandidateResult>>> GetAdmissionCandidateResult(string candidateCategoryId);
    }
}
