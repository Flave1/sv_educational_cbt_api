using CBT.BLL.Constants;
using CBT.BLL.Services.Questions;
using CBT.Contracts;
using CBT.Contracts.CandidateAnswer;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.Contracts.Question;
using CBT.DAL;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Question;
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
        private readonly DataContext _context;
        private readonly IQuestionService _questionService;
        private readonly IHttpContextAccessor _accessor;

        public CandidateAnswerService(DataContext context, IQuestionService questionService, IHttpContextAccessor accessor)
        {
            _context = context;
            _questionService = questionService;
            _accessor = accessor;
        }
        public async Task<APIResponse<CreateCandidateAnswer>> CreateCandidateAnswer(CreateCandidateAnswer request)
        {
            var res = new APIResponse<CreateCandidateAnswer>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var question = await _context.Question.Where(m => m.QuestionId == Guid.Parse(request.QuestionId) && m.ClientId == clientId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "QuestionId doesn't exist";
                    return res;
                }
                var candidate = await _context.Candidate.Where(m => m.CandidateId == Guid.Parse(request.CandidateId) && m.ClientId == clientId).FirstOrDefaultAsync();
                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }

                string answers = "";
                foreach(int item in request.Answers)
                {
                    answers = $"{answers}{item},";
                }

                var candidateAnswer = new CandidateAnswer
                {
                    QuestionId = Guid.Parse(request.QuestionId),
                    CandidateId = Guid.Parse(request.CandidateId),
                    Answers = answers,
                    ClientId = clientId
                };

                _context.CandidateAnswer.Add(candidateAnswer);
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

        public async Task<APIResponse<IEnumerable<SelectCandidateAnswer>>> GetAllCandidateAnswers()
        {
            var res = new APIResponse<IEnumerable<SelectCandidateAnswer>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var answers = await _context.CandidateAnswer
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.ClientId == clientId).ToListAsync();

                var result = answers.Select( a => new SelectCandidateAnswer
                {
                        AnswerId = a.AnswerId.ToString(),
                        QuestionId = a.QuestionId.ToString(),
                        CandidateId = a.CandidateId.ToString(),
                        Answers = _questionService.GetQuestion(a.QuestionId).Result.Result.Answers.ToArray()
                });

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
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var answer = await _context.CandidateAnswer
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.AnswerId == Id && d.ClientId == clientId).FirstOrDefaultAsync();

                if (answer == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var result = new SelectCandidateAnswer
                {
                    AnswerId = answer.AnswerId.ToString(),
                    QuestionId = answer.QuestionId.ToString(),
                    CandidateId = answer.CandidateId.ToString(),
                    Answers = _questionService.GetQuestion(answer.QuestionId).Result.Result.Answers.ToArray()
                };

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
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var answer = await _context.CandidateAnswer.Where(m => m.AnswerId == request.AnswerId && m.ClientId == clientId).FirstOrDefaultAsync();
                if (answer == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AnswerId doesn't exist";
                    return res;
                }

                var question = await _context.Question.Where(m => m.QuestionId == Guid.Parse(request.QuestionId)).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "QuestionId doesn't exist";
                    return res;
                }

                var candidate = await _context.Candidate.Where(m => m.CandidateId == Guid.Parse(request.CandidateId)).FirstOrDefaultAsync();
                if (candidate == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "CandidateId doesn't exist";
                    return res;
                }

                string answers = "";
                foreach (int item in request.Answers)
                {
                    answers = $"{answers}{item},";
                }

                answer.QuestionId = Guid.Parse(request.QuestionId);
                answer.CandidateId = Guid.Parse(request.CandidateId);
                answer.Answers = answers;
               
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
        public async Task<APIResponse<bool>> DeleteCandidateAnswer(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var answer = await _context.CandidateAnswer.Where(d => d.Deleted != true && d.AnswerId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (answer == null)
                {
                    res.Message.FriendlyMessage = "AnswerId doesn't exist";
                    res.IsSuccessful = true;
                    return res;
                }

                answer.Deleted = true;
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
