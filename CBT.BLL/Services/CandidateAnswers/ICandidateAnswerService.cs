using CBT.Contracts.Common;
using CBT.Contracts.Questions;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.CandidateAnswers;
using CBT.BLL.Wrappers;
using CBT.BLL.Filters;

namespace CBT.BLL.Services.CandidateAnswers
{
    public interface ICandidateAnswerService
    {
        Task<APIResponse<CreateCandidateAnswer>> SubmitCandidateAnswer(CreateCandidateAnswer request);
        Task<APIResponse<bool>> SubmitAllCandidateAnswer(SubmitAllAnswers request);
        Task<APIResponse<PagedResponse<List<SelectCandidateAnswer>>>> GetAllCandidateAnswers(PaginationFilter filter);
        Task<APIResponse<SelectCandidateAnswer>> GetCandidateAnswer(Guid Id);
        Task<APIResponse<bool>> DeleteCandidateAnswer(SingleDelete request);
        Task<APIResponse<bool>> SubmitExamination();
    }
}
