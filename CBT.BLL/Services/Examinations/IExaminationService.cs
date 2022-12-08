using CBT.Contracts.Category;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.BLL.Filters;
using CBT.BLL.Wrappers;

namespace CBT.BLL.Services.Examinations
{
    public interface IExaminationService
    {
        Task<APIResponse<CreateExamination>> CreateExamination(CreateExamination request);
        Task<APIResponse<PagedResponse<List<SelectExamination>>>> GetAllExamination(PaginationFilter filter, int examType);
        Task<APIResponse<SelectExamination>> GetExamination(Guid Id);
        Task<APIResponse<UpdateExamination>> UpdateExamination(UpdateExamination request);
        Task<APIResponse<bool>> DeleteExamination(SingleDelete request);
        Task<APIResponse<PagedResponse<List<SelectExamination>>>> GetExaminationByStatus(PaginationFilter filter, int examStatus);
    }
}
