using CBT.Contracts.Category;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Examination;
using CBT.Contracts.Common;

namespace CBT.BLL.Services.Examinations
{
    public interface IExaminationService
    {
        Task<APIResponse<CreateExamination>> CreateExamination(CreateExamination request, Guid clientId, int userType);
        Task<APIResponse<List<SelectExamination>>> GetAllExamination();
        Task<APIResponse<SelectExamination>> GetExamination(Guid Id);
        Task<APIResponse<UpdateExamination>> UpdateExamination(UpdateExamination request);
        Task<APIResponse<bool>> DeleteExamination(SingleDelete request);
    }
}
