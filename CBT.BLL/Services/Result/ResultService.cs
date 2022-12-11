using CBT.BLL.Constants;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Result;
using CBT.DAL;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Result
{
    public class ResultService : IResultService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public ResultService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        public async Task<APIResponse<SelectResult>> GetResult()
        {
            var res = new APIResponse<SelectResult>();
            try
            {
                var examinationId = Guid.Parse(accessor.HttpContext.Items["examinationId"].ToString());
                var candidateId_regNo = accessor.HttpContext.Items["candidateId_regNo"].ToString();

                var examination = await context.Examination?.Where(x => x.ExaminationId == examinationId)?.FirstOrDefaultAsync();

                var questionIds = await context.Question?.Where(x => x.ExaminationId == examinationId)
                    .Select(x => x.QuestionId)
                    .ToListAsync();

                int totalScore = 0;
                foreach (var item in questionIds)
                {
                    var answer = await context.Question?.Where(x => x.QuestionId == item)?.FirstOrDefaultAsync();
                    var candidateAnswer = await context.CandidateAnswer?.Where(x => x.QuestionId == item && x.CandidateId == candidateId_regNo)?.FirstOrDefaultAsync();

                    if (answer?.Answers == candidateAnswer?.Answers)
                    {
                        totalScore += answer.Mark;
                    }

                }

                string status = "";

                if(examination.ExaminationType == (int)ExaminationType.InternalExam)
                {
                    status = totalScore >= examination.ExamScore ? "Passed" : "Failed";
                }
                else
                {
                    status = totalScore >= examination.PassMark ? "Passed" : "Failed";
                }

                var result = new SelectResult
                {
                    CandidateId = candidateId_regNo,
                    ExaminationName = examination.ExamName_Subject,
                    TotalScore = totalScore,
                    Status = status
                };

                res.IsSuccessful = true;
                res.Result = result;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;

            }
            catch(Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }

        }
    }
}
