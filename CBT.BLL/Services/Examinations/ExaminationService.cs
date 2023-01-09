using CBT.BLL.Constants;
using CBT.BLL.Filters;
using CBT.BLL.Services.Pagination;
using CBT.BLL.Services.Session;
using CBT.BLL.Utilities;
using CBT.BLL.Wrappers;
using CBT.Contracts;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.DAL;
using CBT.DAL.Models.Examinations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CBT.BLL.Services.Examinations
{
    public class ExaminationService : IExaminationService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly ISessionService sessionService;
        private readonly IPaginationService paginationService;
        private readonly DateTime localTime;
        public ExaminationService(DataContext context, IHttpContextAccessor accessor, ISessionService sessionService, IPaginationService paginationService,
            IUtilityService utilityService)
        {
            this.context = context;
            this.accessor = accessor;
            this.sessionService = sessionService;
            this.paginationService = paginationService;
            this.localTime = utilityService.GetCurrentLocalDateTime();
        }
        public async Task<APIResponse<CreateExamination>> CreateExamination(CreateExamination request)
        {
            var res = new APIResponse<CreateExamination>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

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
                    var service = await sessionService.GetActiveSession(request.ExamScore, request.UseAsExamScore, request.UseAsAssessmentScore);
                    if (service.Result == null)
                    {
                        res.IsSuccessful = false;
                        res.Message.FriendlyMessage = service.Message.FriendlyMessage;
                        return res;
                    }
                    asExamScoreSessionAndTerm = $"{service.Result.SessionId}|{service.Result.SessionTermId}";
                    asAssessmentScoreSessionAndTerm = $"{service.Result.SessionId}|{service.Result.SessionTermId}";
                }

                var result = UtilTools.GenerateExaminationId();
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
                    UploadToSmpAsExam = request.UploadToSmpAsExam,
                    UploadToSmpAsAssessment = request.UploadToSmpAsAssessment,
                    ExaminationNo = result.Keys.First(),
                    CandidateExaminationId = result.Values.First(),
                    Status = request.Status,
                    ExaminationType = request.ExaminationType,
                    PassMark = request.PassMark,
                    ProductBaseurlSuffix = accessor?.HttpContext?.Items["productBaseurlSuffix"].ToString() ?? ""
                };

                context.Examination.Add(examination);
                await context.SaveChangesAsync();
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

        public async Task<APIResponse<PagedResponse<List<SelectExamination>>>> GetAllExamination(PaginationFilter filter, int examType)
        {
            var res = new APIResponse<PagedResponse<List<SelectExamination>>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var query = context.Examination
                    .Where(d => d.Deleted != true && d.ExaminationType == examType && d.ClientId == clientId)
                    .Include(q => q.Question);

                var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).OrderByDescending(s => s.CreatedOn)
                    .Select(db => new SelectExamination(db,localTime)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);

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

        public async Task<APIResponse<PagedResponse<List<SelectExamination2>>>> GetAllExamination2(PaginationFilter filter, string sessionClassId)
        {
            var res = new APIResponse<PagedResponse<List<SelectExamination2>>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var query = context.Examination
                    .Where(d => d.Deleted != true && d.ExaminationType == (int)ExaminationType.InternalExam && d.ClientId == clientId && d.CandidateCategoryId_ClassId == sessionClassId);
                //.Include(q => q.Question);

                var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).OrderByDescending(s => s.CreatedOn)
                    .Select(db => new SelectExamination2(db, localTime)).ToListAsync();
                res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);

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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var result = await context.Examination
                    .Where(d => d.Deleted != true && d.ExaminationId == Id && d.ClientId == clientId)
                    .Include(q => q.Question)
                    .Select(db => new SelectExamination(db, localTime)).FirstOrDefaultAsync();

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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var result = await context.Examination.Where(m => m.ExaminationId == request.ExaminationId && m.ClientId == clientId).FirstOrDefaultAsync();
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
                    var service = await sessionService.GetActiveSession(request.ExamScore, request.UseAsExamScore, request.UseAsAssessmentScore);
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
                result.StartTime = DateTime.Parse(request.StartTime);
                result.EndTime = DateTime.Parse(request.EndTime);
                result.Instruction = request.Instruction;
                result.ShuffleQuestions = request.ShuffleQuestions;
                result.UseAsExamScore = request.UseAsExamScore;
                result.UseAsAssessmentScore = request.UseAsAssessmentScore;
                result.AsExamScoreSessionAndTerm = asExamScoreSessionAndTerm;
                result.AsAssessmentScoreSessionAndTerm = asAssessmentScoreSessionAndTerm;
                result.UploadToSmpAsExam = request.UploadToSmpAsExam;
                result.UploadToSmpAsAssessment = request.UploadToSmpAsAssessment;
                result.Status = request.Status;
                result.ExaminationType = request.ExaminationType;
                result.PassMark = request.PassMark;
                result.ProductBaseurlSuffix = accessor?.HttpContext?.Items["productBaseurlSuffix"].ToString() ?? "";

                await context.SaveChangesAsync();
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var examination = await context.Examination.Where(d => d.Deleted != true && d.ExaminationId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (examination == null)
                {
                    res.Message.FriendlyMessage = "ExaminationId does not exist";
                    res.IsSuccessful = false;
                    return res;
                }

                examination.Deleted = true;
                await context.SaveChangesAsync();

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

        public async Task<APIResponse<PagedResponse<List<SelectExamination>>>> GetExaminationByStatus(PaginationFilter filter, int examStatus)
        {
            var res = new APIResponse<PagedResponse<List<SelectExamination>>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                if(examStatus == (int)ExaminationStatus.InProgress)
                {
                    var query = context.Examination
                    .Where(d => d.Deleted != true && (d.StartTime <= localTime && d.EndTime > localTime) && d.ClientId == clientId)
                    .Include(q=>q.Question)
                    .OrderByDescending(s => s.CreatedOn);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(db => new SelectExamination(db, localTime)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
                }

                if (examStatus == (int)ExaminationStatus.Concluded)
                {
                    var query = context.Examination
                    .Where(d => d.Deleted != true && (d.StartTime < localTime && d.EndTime < localTime) && d.ClientId == clientId)
                    .Include(q => q.Question)
                    .OrderByDescending(s => s.CreatedOn);

                    var totalRecord = query.Count();
                    var result = await paginationService.GetPagedResult(query, filter).Select(db => new SelectExamination(db, localTime)).ToListAsync();
                    res.Result = paginationService.CreatePagedReponse(result, filter, totalRecord);
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
