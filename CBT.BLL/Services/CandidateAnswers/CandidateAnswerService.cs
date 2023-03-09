using CBT.BLL.Constants;
using CBT.BLL.Filters;
using CBT.BLL.Services.Pagination;
using CBT.BLL.Services.Questions;
using CBT.BLL.Wrappers;
using CBT.Contracts;
using CBT.Contracts.CandidateAnswers;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Questions;
using CBT.DAL;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Examinations;
using CBT.DAL.Models.Questions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CBT.BLL.Services.CandidateAnswers
{
    public class CandidateAnswerService : ICandidateAnswerService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IPaginationService paginationService;

        public CandidateAnswerService(DataContext context, IHttpContextAccessor accessor, IPaginationService paginationService)
        {
            this.context = context;
            this.accessor = accessor;
            this.paginationService = paginationService;
        }
        public async Task<APIResponse<CreateCandidateAnswer>> SubmitCandidateAnswer(CreateCandidateAnswer request)
        {
            var res = new APIResponse<CreateCandidateAnswer>();
            try
            {
                var candidateAnswers = await context.CandidateAnswer.FirstOrDefaultAsync(a => a.QuestionId == Guid.Parse(request.QuestionId) && a.CandidateId == request.CandidateId);

                if(candidateAnswers == null)
                {
                    var newCandidateAnswer = new CandidateAnswer
                    {
                        QuestionId = Guid.Parse(request.QuestionId),
                        CandidateId = request.CandidateId,
                        Answers = String.Join(",", request.Answers)
                    };
                    context.CandidateAnswer.Add(newCandidateAnswer);
                }
                else
                {
                    candidateAnswers.Answers = String.Join(",", request.Answers);
                }

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
        public async Task<APIResponse<bool>> SubmitAllCandidateAnswer(SubmitAllAnswers request)
        {
            var res = new APIResponse<bool>();
            try
            {
                foreach (var item in request.Answers)
                {
                    var candidateAnswers = await context.CandidateAnswer.FirstOrDefaultAsync(a => a.QuestionId == Guid.Parse(item.QuestionId) && a.CandidateId == request.CandidateId);

                    if(candidateAnswers == null)
                    {
                        var newCandidateAnswer = new CandidateAnswer
                        {
                            QuestionId = Guid.Parse(item.QuestionId),
                            CandidateId = request.CandidateId,
                            Answers = String.Join(",", item.Answers)
                        };
                        context.CandidateAnswer.Add(newCandidateAnswer);
                    }
                    else
                    {
                        candidateAnswers.Answers = String.Join(",", item.Answers);
                    }
                    
                }

                await context.SaveChangesAsync();
                res.Result = true;
                res.Message.FriendlyMessage = Messages.Created;
                res.IsSuccessful = true;
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
        public async Task<APIResponse<PagedResponse<List<SelectCandidateAnswer>>>> GetAllCandidateAnswers(PaginationFilter filter)
        {
            var res = new APIResponse<PagedResponse<List<SelectCandidateAnswer>>>();
            try
            {

                var candidateId_regNo = accessor.HttpContext.Items["candidateId_regNo"].ToString();
                var examinationId = Guid.Parse(accessor.HttpContext.Items["examinationId"].ToString());

                var questions = await context.Question.Where(x => x.ExaminationId == examinationId).Select(x=>x.QuestionId).ToListAsync();
                var query = context.CandidateAnswer
                    .Where(d => d.Deleted != true && questions.Contains(d.QuestionId) && d.CandidateId == candidateId_regNo)
                    .Include(q => q.Question)
                    .OrderByDescending(s => s.CreatedOn);

                var totalRecord = query.Count();
                var result = await paginationService.GetPagedResult(query, filter).Select(db => new SelectCandidateAnswer(db)).ToListAsync();
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

        public async Task<APIResponse<SelectCandidateAnswer>> GetCandidateAnswer(Guid Id)
        {
            var res = new APIResponse<SelectCandidateAnswer>();
            try
            {
                var result = await context.CandidateAnswer
                    .Where(d => d.Deleted != true && d.AnswerId == Id)
                    .Include(q => q.Question)
                    .Select(db => new SelectCandidateAnswer(db))
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

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
        public async Task<APIResponse<bool>> DeleteCandidateAnswer(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var answer = await context.CandidateAnswer.Where(d => d.Deleted != true && d.AnswerId == Guid.Parse(request.Item)).FirstOrDefaultAsync();
                if (answer == null)
                {
                    res.Message.FriendlyMessage = "AnswerId doesn't exist";
                    res.IsSuccessful = false;
                    return res;
                }

                answer.Deleted = true;
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

        public async Task<APIResponse<bool>> SubmitExamination()
        {
            var res = new APIResponse<bool>();
            try
            {
                var candidateId_regNo = accessor.HttpContext.Items["candidateId_regNo"].ToString();
                var examinationId = Guid.Parse(accessor.HttpContext.Items["examinationId"].ToString());

                var examination = await context.Examination?.FirstOrDefaultAsync(a => a.ExaminationId == examinationId);
                
                if(string.IsNullOrEmpty(examination.CandidateIds))
                {
                    examination.CandidateIds += candidateId_regNo;
                }
                else
                {
                    if (!examination.CandidateIds.Split(",").Contains(candidateId_regNo))
                    {
                        examination.CandidateIds += $",{candidateId_regNo}";
                    }
                }
                await SetExaminerDetails(examination);
                
                await context.SaveChangesAsync();
                res.Result = true;
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
        private async Task SetExaminerDetails(Examination examination)
        {
            accessor.HttpContext.Items["userId"] = examination.ClientId;
            accessor.HttpContext.Items["productBaseurlSuffix"] = examination.ProductBaseurlSuffix;
            if(examination.UserType == (int)UserType.NonSMSUser)
            {
                accessor.HttpContext.Items["smsClientId"] = string.Empty;
            }
            else
            {
                accessor.HttpContext.Items["smsClientId"] = examination.SmsClientId; 
            }
        }
    }
}
