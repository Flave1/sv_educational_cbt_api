using CBT.Contracts;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Category
{
    public interface ICandidateCategoryService
    {
        Task<APIResponse<CreateCandidateCategory>> CreateCandidateCategory(CreateCandidateCategory request);
        Task<APIResponse<List<SelectCandidateCategory>>> GetAllCandidateCategory();
        Task<APIResponse<SelectCandidateCategory>> GetCandidateCategory(Guid Id);
        Task<APIResponse<UpdateCandidateCategory>> UpdateCandidateCategory(UpdateCandidateCategory request);
        Task<APIResponse<bool>> DeleteCandidateCategory(SingleDelete request);
    }
}
