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

        public CandidateAnswerService(DataContext context, IQuestionService questionService)
        {
            _context = context;
            _questionService = questionService;
        }
        public async Task<APIResponse<CreateCandidateAnswer>> CreateCandidateAnswer(CreateCandidateAnswer request, Guid clientId)
        {
            var res = new APIResponse<CreateCandidateAnswer>();
            try
            {
                var question = await _context.Question.Where(m => m.QuestionId == request.QuestionId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "QuestionId doesn't exist";
                    return res;
                }
                var candidate = await _context.Candidate.Where(m => m.CandidateId == request.CandidateId).FirstOrDefaultAsync();
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
                    QuestionId = request.QuestionId,
                    CandidateId = request.CandidateId,
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
                var answer = await _context.CandidateAnswer
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true).ToListAsync();

                var result = answer.Select( a => new SelectCandidateAnswer
                {
                        AnswerId = a.AnswerId,
                        QuestionId = a.QuestionId,
                        CandidateId = a.CandidateId,
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

                var answer = await _context.CandidateAnswer
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.AnswerId == Id).FirstOrDefaultAsync();

                if (answer == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var result = new SelectCandidateAnswer
                {
                    AnswerId = answer.AnswerId,
                    QuestionId = answer.QuestionId,
                    CandidateId = answer.CandidateId,
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
                var answer = await _context.CandidateAnswer.Where(m => m.AnswerId == request.AnswerId).FirstOrDefaultAsync();
                if (answer == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "AnswerId doesn't exist";
                    return res;
                }

                var question = await _context.Question.Where(m => m.QuestionId == request.QuestionId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "QuestionId doesn't exist";
                    return res;
                }

                var candidate = await _context.Candidate.Where(m => m.CandidateId == request.CandidateId).FirstOrDefaultAsync();
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

                answer.QuestionId = request.QuestionId;
                answer.CandidateId = request.CandidateId;
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
                var answer = await _context.CandidateAnswer.Where(d => d.Deleted != true && d.AnswerId == Guid.Parse(request.Item)).FirstOrDefaultAsync();
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
