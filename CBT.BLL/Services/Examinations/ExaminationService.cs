using CBT.BLL.Constants;
using CBT.BLL.Services.Class;
using CBT.BLL.Services.Session;
using CBT.Contracts;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
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
        private readonly ISessionService _sessionService;

        public ExaminationService(DataContext context, IHttpContextAccessor accessor, ISessionService sessionService)
        {
            _context = context;
            _accessor = accessor;
            _sessionService = sessionService;
        }
        public async Task<APIResponse<CreateExamination>> CreateExamination(CreateExamination request)
        {
            var res = new APIResponse<CreateExamination>();

            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["smsClientId"].ToString());

                TimeSpan duration;
                if (!TimeSpan.TryParse(request.Duration, out duration))
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Error! Duration format is invalid";
                    return res;
                }
                string asExamScoreSessionAndTerm = "";
                string asAssessmentScoreSessionAndTerm = "";

                if (request.UseAsExamScore || request.UseAsAssessmentScore)
                {
                    var service = await _sessionService.GetActiveSession(request.ExamScore, request.UseAsExamScore, request.UseAsAssessmentScore);
                    if (service.Result == null)
                    {
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = service.Message.FriendlyMessage;
                        return res;
                    }
                    asExamScoreSessionAndTerm = $"{service.Result.SessionId}|{service.Result.SessionTermId}";
                    asAssessmentScoreSessionAndTerm = $"{service.Result.SessionId}|{service.Result.SessionTermId}";
                }

                var examination = new Examination
                {
                    ExamName_SubjectId = request.ExamName_SubjectId,
                    ExamName_Subject = request.ExamName_Subject,
                    CandidateCategoryId_ClassId = request.CandidateCategoryId_ClassId,
                    CandidateCategory_Class = request.CandidateCategory_Class,
                    ExamScore = request.ExamScore,
                    Duration = duration,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Instruction = request.Instruction,
                    ShuffleQuestions = request.ShuffleQuestions,
                    UseAsExamScore = request.UseAsExamScore,
                    UseAsAssessmentScore = request.UseAsAssessmentScore,
                    AsExamScoreSessionAndTerm = asExamScoreSessionAndTerm,
                    AsAssessmentScoreSessionAndTerm = asAssessmentScoreSessionAndTerm,
                    Status = request.Status,
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
                var clientId = Guid.Parse(_accessor.HttpContext.Items["smsClientId"].ToString());

                var result = await _context.Examination
                .OrderByDescending(s => s.CreatedOn)
                .Where(d => d.Deleted != true && d.ClientId == clientId)
                .Select(db => new SelectExamination(db)).ToListAsync();

                res.Result = result;
                res.IsSuccessful = true;
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
                var clientId = Guid.Parse(_accessor.HttpContext.Items["smsClientId"].ToString());

                var result = await _context.Examination
                .OrderByDescending(s => s.CreatedOn)
                .Where(d => d.Deleted != true && d.ExaminationId == Id && d.ClientId == clientId)
                .Select(db => new SelectExamination(db)).FirstOrDefaultAsync();

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
                var clientId = Guid.Parse(_accessor.HttpContext.Items["smsClientId"].ToString());

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

                string asExamScoreSessionAndTerm = "";
                string asAssessmentScoreSessionAndTerm = "";

                if (request.UseAsExamScore || request.UseAsAssessmentScore)
                {
                    var service = await _sessionService.GetActiveSession(request.ExamScore, request.UseAsExamScore, request.UseAsAssessmentScore);
                    if (service.Result == null)
                    {
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = service.Message.FriendlyMessage;
                        return res;
                    }
                    asExamScoreSessionAndTerm = $"{service.Result.SessionId}|{service.Result.SessionTermId}";
                    asAssessmentScoreSessionAndTerm = $"{service.Result.SessionId}|{service.Result.SessionTermId}";
                }

                result.ExamName_SubjectId = request.ExamName_SubjectId;
                result.ExamName_Subject = request.ExamName_Subject;
                result.CandidateCategoryId_ClassId = request.CandidateCategoryId_ClassId;
                result.CandidateCategory_Class = request.CandidateCategory_Class;
                result.ExamScore = request.ExamScore;
                result.Duration = duration;
                result.StartTime = request.StartTime;
                result.EndTime = request.EndTime;
                result.Instruction = request.Instruction;
                result.ShuffleQuestions = request.ShuffleQuestions;
                result.UseAsExamScore = request.UseAsExamScore;
                result.UseAsAssessmentScore = request.UseAsAssessmentScore;
                result.AsExamScoreSessionAndTerm = asExamScoreSessionAndTerm;
                result.AsAssessmentScoreSessionAndTerm = asAssessmentScoreSessionAndTerm;
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
                var clientId = Guid.Parse(_accessor.HttpContext.Items["smsClientId"].ToString());

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

        public async Task<APIResponse<List<SelectExamination>>> GetExaminationByStatus(int examStatus)
        {
            var res = new APIResponse<List<SelectExamination>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["smsClientId"].ToString());

                if(examStatus == (int)ExaminationStatus.InProgress)
                {
                    var result = await _context.Examination
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && (d.StartTime <= DateTime.Now && d.EndTime > DateTime.Now) && d.ClientId == clientId)
                    .Select(db => new SelectExamination(db)).ToListAsync();

                    res.Result = result;
                }

                if (examStatus == (int)ExaminationStatus.Concluded)
                {
                    var result = await _context.Examination
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && (d.StartTime < DateTime.Now && d.EndTime < DateTime.Now) && d.ClientId == clientId)
                    .Select(db => new SelectExamination(db)).ToListAsync();

                    res.Result = result;
                }

                res.IsSuccessful = true;
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
    }
}
