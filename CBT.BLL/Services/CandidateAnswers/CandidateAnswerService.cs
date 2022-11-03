using CBT.BLL.Constants;
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

        public CandidateAnswerService(DataContext context)
        {
            _context = context;
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

                var candidateAnswer = new CandidateAnswer
                {
                    QuestionId = request.QuestionId,
                    CandidateId = request.CandidateId,
                    Answers = request.Answers,
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
                var result = await _context.CandidateAnswer
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true).Select(a => new SelectCandidateAnswer
                    {
                        AnswerId = a.AnswerId,
                        QuestionId = a.QuestionId,
                        CandidateId = a.CandidateId,
                        Answers = a.Answers
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

        public async Task<APIResponse<SelectCandidateAnswer>> GetCandidateAnswer(Guid Id)
        {
            var res = new APIResponse<SelectCandidateAnswer>();
            try
            {
                var answer = await _context.CandidateAnswer.Where(m => m.AnswerId == Id).FirstOrDefaultAsync();
                if (answer == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var result = await _context.CandidateAnswer
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.AnswerId == Id).Select(a => new SelectCandidateAnswer
                    {
                        AnswerId = a.AnswerId,
                        QuestionId = a.QuestionId,
                        CandidateId = a.CandidateId,
                        Answers = a.Answers
                    }).FirstOrDefaultAsync();

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

                answer.QuestionId = request.QuestionId;
                answer.CandidateId = request.CandidateId;
                answer.Answers = request.Answers;
               
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
