using CBT.BLL.Constants;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Examination;
using CBT.DAL;
using CBT.DAL.Models.Candidates;
using CBT.DAL.Models.Examinations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Examinations
{
    public class ExaminationService : IExaminationService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _accessor;

        public ExaminationService(DataContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public async Task<APIResponse<CreateExamination>> CreateExamination(CreateExamination request)
        {
            var res = new APIResponse<CreateExamination>();

            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var userType = int.Parse(_accessor.HttpContext.Items["userType"].ToString());

                TimeSpan duration;
                if (!TimeSpan.TryParse(request.Duration, out duration))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Error! Duration format is invalid";
                    return res;
                }
                var examination = new Examination
                {
                    ExamName_SubjectId = request.ExamName_SubjectId,
                    ExamName_Subject = request.ExamName_Subject,
                    CandidateCategoryId_ClassId = request.CandidateCategoryId_ClassId,
                    Duration = duration,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Instruction = request.Instruction,
                    ShuffleQuestions = request.ShuffleQuestions,
                    UseAsExamScore = request.UseAsExamScore,
                    UseAsAssessmentScore = request.UseAsAssessmentScore,
                    AsExamScoreSessionAndTerm = request.AsExamScoreSessionAndTerm,
                    AsAssessmentScoreSessionAndTerm = request.AsAssessmentScoreSessionAndTerm,
                    Status = request.Status,
                    UserType = userType,
                    ClientId = clientId
                };

                _context.Examination.Add(examination);
                await _context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Created;
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

        public async Task<APIResponse<List<SelectExamination>>> GetAllExamination()
        {
            var res = new APIResponse<List<SelectExamination>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.Examination
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.ClientId == clientId).Select(a => new SelectExamination
                    {
                        ExaminationId = a.ExaminationId.ToString(),
                        ExamName_SubjectId = a.ExamName_SubjectId,
                        ExamName_Subject = a.ExamName_Subject,
                        CandidateCategoryId_ClassId = a.CandidateCategoryId_ClassId,
                        Duration = a.Duration,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Instruction = a.Instruction,
                        ShuffleQuestions = a.ShuffleQuestions,
                        UseAsExamScore = a.UseAsExamScore,
                        UseAsAssessmentScore = a.UseAsAssessmentScore,
                        AsExamScoreSessionAndTerm = a.AsExamScoreSessionAndTerm,
                        AsAssessmentScoreSessionAndTerm = a.AsAssessmentScoreSessionAndTerm
                    }).ToListAsync();

                res.IsSuccessful = true;
                res.Result = result;
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

        public async Task<APIResponse<SelectExamination>> GetExamination(Guid Id)
        {
            var res = new APIResponse<SelectExamination>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.Examination
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.ExaminationId == Id && d.ClientId == clientId).Select(a => new SelectExamination
                    {
                        ExaminationId = a.ExaminationId.ToString(),
                        ExamName_SubjectId = a.ExamName_SubjectId,
                        ExamName_Subject = a.ExamName_Subject,
                        CandidateCategoryId_ClassId = a.CandidateCategoryId_ClassId,
                        Duration = a.Duration,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Instruction = a.Instruction,
                        ShuffleQuestions = a.ShuffleQuestions,
                        UseAsExamScore = a.UseAsExamScore,
                        UseAsAssessmentScore = a.UseAsAssessmentScore,
                        AsExamScoreSessionAndTerm = a.AsExamScoreSessionAndTerm,
                        AsAssessmentScoreSessionAndTerm = a.AsAssessmentScoreSessionAndTerm
                    }).FirstOrDefaultAsync();

                if (result == null)
                {
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                }
                else
                {
                    res.Message.FriendlyMessage = Messages.GetSuccess;
                }

                res.IsSuccessful = true;
                res.Result = result;
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

        public async Task<APIResponse<UpdateExamination>> UpdateExamination(UpdateExamination request)
        {
            var res = new APIResponse<UpdateExamination>();

            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var result = await _context.Examination.Where(m => m.ExaminationId == request.ExaminationId && m.ClientId == clientId).FirstOrDefaultAsync();
                if (result == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "ExaminationId doesn't exist";
                    return res;
                }

                TimeSpan duration;
                if (!TimeSpan.TryParse(request.Duration, out duration))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Error! Duration format is invalid";
                    return res;
                }
                result.ExamName_SubjectId = request.ExamName_SubjectId;
                result.ExamName_Subject = request.ExamName_Subject;
                result.CandidateCategoryId_ClassId = request.CandidateCategoryId_ClassId;
                result.Duration = duration;
                result.StartTime = request.StartTime;
                result.EndTime = request.EndTime;
                result.Instruction = request.Instruction;
                result.ShuffleQuestions = request.ShuffleQuestions;
                result.UseAsExamScore = request.UseAsExamScore;
                result.UseAsAssessmentScore = request.UseAsAssessmentScore;
                result.AsExamScoreSessionAndTerm = request.AsExamScoreSessionAndTerm;
                result.AsAssessmentScoreSessionAndTerm = request.AsAssessmentScoreSessionAndTerm;
                result.Status = request.Status;

                await _context.SaveChangesAsync();
                res.Result = request;
                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.Updated;
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
        public async Task<APIResponse<bool>> DeleteExamination(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var examination = await _context.Examination.Where(d => d.Deleted != true && d.ExaminationId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (examination == null)
                {
                    res.Message.FriendlyMessage = "ExaminationId does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                examination.Deleted = true;
                await _context.SaveChangesAsync();

                res.IsSuccessful = true;
                res.Message.FriendlyMessage = Messages.DeletedSuccess;
                res.Result = true;
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
    }
}
