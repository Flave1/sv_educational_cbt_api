using CBT.BLL.Constants;
using CBT.BLL.Services.Questions;
using CBT.Contracts;
using CBT.Contracts.CandidateAnswers;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Questions;
using CBT.DAL;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Questions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.CandidateAnswers
{
    public class CandidateAnswerService : ICandidateAnswerService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public CandidateAnswerService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        public async Task<APIResponse<CreateCandidateAnswer>> SubmitCandidateAnswer(CreateCandidateAnswer request)
        {
            var res = new APIResponse<CreateCandidateAnswer>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var candidate = await context.Candidate.FirstOrDefaultAsync(m => m.Id == Guid.Parse(request.CandidateId) && m.ClientId == clientId);
                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }

                string answers = String.Join(",", request.Answers);

                var candidateAnswer = new CandidateAnswer
                {
                    QuestionId = Guid.Parse(request.QuestionId),
                    CandidateId = Guid.Parse(request.CandidateId),
                    Answers = answers,
                    ClientId = clientId
                };

                context.CandidateAnswer.Add(candidateAnswer);
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var candidate = await context.Candidate.FirstOrDefaultAsync(m => m.Id == Guid.Parse(request.CandidateId) && m.ClientId == clientId);

                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }

                foreach (var item in request.Answers)
                {
                    string answers = String.Join(",", item.Answers);
                    var candidateAnswer = new CandidateAnswer
                    {
                        QuestionId = Guid.Parse(item.QuestionId),
                        CandidateId = candidate.Id,
                        Answers = answers,
                        ClientId = clientId
                    };
                    context.CandidateAnswer.Add(candidateAnswer);
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
        public async Task<APIResponse<IEnumerable<SelectCandidateAnswer>>> GetAllCandidateAnswers()
        {
            var res = new APIResponse<IEnumerable<SelectCandidateAnswer>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var result = await context.CandidateAnswer
                    .Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Include(q=>q.Question)
                    .OrderByDescending(s => s.CreatedOn)
                    .Select(db => new SelectCandidateAnswer(db))
                    .ToListAsync();

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

        public async Task<APIResponse<SelectCandidateAnswer>> GetCandidateAnswer(Guid Id)
        {
            var res = new APIResponse<SelectCandidateAnswer>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var result = await context.CandidateAnswer
                    .Where(d => d.Deleted != true && d.AnswerId == Id && d.ClientId == clientId)
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

        public async Task<APIResponse<UpdateCandidateAnswer>> UpdateCandidateAnswer(UpdateCandidateAnswer request)
        {
            var res = new APIResponse<UpdateCandidateAnswer>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var answer = await context.CandidateAnswer.FirstOrDefaultAsync(m => m.AnswerId == request.AnswerId && m.ClientId == clientId);
                if (answer == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AnswerId doesn't exist";
                    return res;
                }

                var candidate = await context.Candidate.Where(m => m.Id == Guid.Parse(request.CandidateId)).FirstOrDefaultAsync();
                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }

                string answers = String.Join(",", request.Answers);

                answer.QuestionId = Guid.Parse(request.QuestionId);
                answer.CandidateId = Guid.Parse(request.CandidateId);
                answer.Answers = answers;
               
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
        public async Task<APIResponse<bool>> DeleteCandidateAnswer(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var answer = await context.CandidateAnswer.Where(d => d.Deleted != true && d.AnswerId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
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
    }
}
