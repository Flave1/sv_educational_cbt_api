using CBT.Contracts;
using CBT.Contracts.Candidate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Candidate
{
    public interface ICandidateCategoryService
    {
        Task<APIResponse<CreateCandidateCategory>> CreateCandidateCategory(CreateCandidateCategory request, Guid clientId, int userType);
        Task<APIResponse<List<SelectCandidateCategory>>> GetAllCandidateCategory();
        Task<APIResponse<SelectCandidateCategory>> GetCandidateCategory(Guid Id);
    }
}
