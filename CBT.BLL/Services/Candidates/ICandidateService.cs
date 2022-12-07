using CBT.Contracts.Category;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.BLL.Filters;
using CBT.BLL.Wrappers;

namespace CBT.BLL.Services.Candidates
{
    public interface ICandidateService
    {
        Task<APIResponse<string>> CreateCandidate(CreateCandidate request);
        Task<APIResponse<PagedResponse<List<SelectCandidates>>>> GetAllCandidates(PaginationFilter filter);
        Task<APIResponse<SelectCandidates>> GetCandidate(string candidateId);
        Task<APIResponse<string>> UpdateCandidate(UpdateCandidate request);
        Task<APIResponse<bool>> DeleteCandidate(SingleDelete request);
        Task<APIResponse<CandidateExamDetails>> LoginByExamId(CandidateLoginExamId request);
        Task<AuthDetails> GenerateAuthenticationToken(string examinationId);
        Task<APIResponse<CandidateLoginDetails>> LoginByEmail(CandidateLoginEmail request);
        Task<APIResponse<CandidateLoginDetails>> LoginByRegNo(CandidateLoginRegNo request);
    }
}
