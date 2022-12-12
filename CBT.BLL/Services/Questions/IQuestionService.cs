using CBT.Contracts.Category;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Questions;
using CBT.Contracts.Common;
using CBT.BLL.Wrappers;
using CBT.BLL.Filters;

namespace CBT.BLL.Services.Questions
{
    public interface IQuestionService
    {
        Task<APIResponse<CreateQuestion>> CreateQuestion(CreateQuestion request);
        Task<APIResponse<PagedResponse<List<SelectQuestion>>>> GetAllQuestions(PaginationFilter filter);
        Task<APIResponse<SelectQuestion>> GetQuestion(Guid Id);
        Task<APIResponse<PagedResponse<List<SelectQuestion>>>> GetQuestionByExamId(PaginationFilter filter, Guid examId);
        Task<APIResponse<UpdateQuestion>> UpdateQuestion(UpdateQuestion request);
        Task<APIResponse<bool>> DeleteQuestion(SingleDelete request);
        Task<APIResponse<PagedResponse<List<SelectCandidateQuestions>>>> GetCandidateQuestions(PaginationFilter filter);
    }
}

