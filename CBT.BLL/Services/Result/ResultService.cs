using CBT.BLL.Constants;
using CBT.BLL.Services.Class;
using CBT.BLL.Services.Student;
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
        private readonly IStudentService studentService;

        public ResultService(DataContext context, IHttpContextAccessor accessor,
            IStudentService studentService)
        {
            this.context = context;
            this.accessor = accessor;
            this.studentService = studentService;
        }

        public async Task<APIResponse<List<SelectAllCandidateResult>>> GetAllCandidateResult(string examinationId)
        {
            var res = new APIResponse<List<SelectAllCandidateResult>>();
            try
            {
                var examination = await context.Examination?.Where(x => x.ExaminationId == Guid.Parse(examinationId))?.FirstOrDefaultAsync();

                var questionIds = context.Question?.Where(x => x.ExaminationId == examination.ExaminationId)
                    .Select(x => x.QuestionId)
                    .ToList();

                if (examination.ExaminationType == (int)ExaminationType.ExternalExam)
                {
                    var candidates = await context.Candidate.Where(x => x.CandidateCategoryId == Guid.Parse(examination.CandidateCategoryId_ClassId))
                   .ToListAsync();

                    var result = candidates.Select(x => new SelectAllCandidateResult
                    {
                        CandidateId = x.CandidateId,
                        CandidateName = $"{x.FirstName} {x.LastName}",
                        ExaminationName = examination.ExamName_Subject,
                        TotalScore = GetTotalScore(questionIds, x.Id.ToString()),
                        Status = GetTotalScore(questionIds, x.Id.ToString()) >= examination.PassMark ? "Passed" : "Failed"
                    }).ToList();

                    res.Result = result;
                }

                if (examination.ExaminationType == (int)ExaminationType.InternalExam)
                {
                    //var result = candidates.Select(x => new SelectAllCandidateResult
                    //{
                    //    CandidateId = x.CandidateId,
                    //    CandidateName = $"{x.FirstName} {x.LastName}",
                    //    ExaminationName = examination.ExamName_Subject,
                    //    TotalScore = GetTotalScore(questionIds, x.Id.ToString()),
                    //    Status = GetTotalScore(questionIds, x.Id.ToString()) >= examination.PassMark ? "Passed" : "Failed"
                    //}).ToList();
                }

                res.IsSuccessful = true;;
                res.Message.FriendlyMessage = Messages.GetSuccess;
                return res;

            }
            catch (Exception ex)
            {
                res.IsSuccessful = false;
                res.Message.FriendlyMessage = Messages.FriendlyException;
                res.Message.TechnicalMessage = ex.ToString();
                return res;
            }
        }

        public async Task<APIResponse<SelectResult>> GetCandidateResult()
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
                string candidateName = "";

                if(examination.ExaminationType == (int)ExaminationType.InternalExam)
                {
                    status = totalScore >= examination.ExamScore ? "Passed" : "Failed";
                    var student = await studentService.GetStudentDetails(candidateId_regNo, examination.ProductBaseurlSuffix);
                    if(student.Result != null)
                    {
                        candidateName = $"{student.Result.FirstName} {student.Result.LastName}";
                    }
                }
                else
                {
                    status = totalScore >= examination.PassMark ? "Passed" : "Failed";
                    var candidate = await context.Candidate?.Where(x => x.Id == Guid.Parse(candidateId_regNo))?.FirstOrDefaultAsync();
                    candidateName = $"{ candidate?.FirstName} {candidate?.LastName}";
                }

                var result = new SelectResult
                {
                    CandidateName = candidateName,
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

        private int GetTotalScore(List<Guid> questionIds, string candidateId_regNo)
        {
            int totalScore = 0;
            foreach (var item in questionIds)
            {
                var answer = context.Question?.Where(x => x.QuestionId == item)?.FirstOrDefault();
                var candidateAnswer = context.CandidateAnswer?.Where(x => x.QuestionId == item && x.CandidateId.ToLower() == candidateId_regNo.ToLower())?.FirstOrDefault();

                if (answer?.Answers == candidateAnswer?.Answers)
                {
                    totalScore += answer.Mark;
                }

            }
            return totalScore;
        }
    }
}
