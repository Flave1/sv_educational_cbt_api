using CBT.Contracts.Category;
using CBT.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBT.Contracts.Questions;
using CBT.Contracts.Common;

namespace CBT.BLL.Services.Questions
{
    public interface IQuestionService
    {
        Task<APIResponse<CreateQuestion>> CreateQuestion(CreateQuestion request);
        Task<APIResponse<IEnumerable<SelectQuestion>>> GetAllQuestions();
        Task<APIResponse<SelectQuestion>> GetQuestion(Guid Id);
        Task<APIResponse<IEnumerable<SelectQuestion>>> GetQuestionByExamId(Guid examId);
        Task<APIResponse<UpdateQuestion>> UpdateQuestion(UpdateQuestion request);
        Task<APIResponse<bool>> DeleteQuestion(SingleDelete request);
    }
}

