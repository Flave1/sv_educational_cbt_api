using CBT.Contracts.Common;
using CBT.Contracts.Questions;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.CandidateAnswers;

namespace CBT.BLL.Services.CandidateAnswers
{
    public interface ICandidateAnswerService
    {
        Task<APIResponse<CreateCandidateAnswer>> SubmitCandidateAnswer(CreateCandidateAnswer request);
        Task<APIResponse<bool>> SubmitAllCandidateAnswer(SubmitAllAnswers request);
        Task<APIResponse<IEnumerable<SelectCandidateAnswer>>> GetAllCandidateAnswers();
        Task<APIResponse<SelectCandidateAnswer>> GetCandidateAnswer(Guid Id);
        Task<APIResponse<UpdateCandidateAnswer>> UpdateCandidateAnswer(UpdateCandidateAnswer request);
        Task<APIResponse<bool>> DeleteCandidateAnswer(SingleDelete request);
    }
}
