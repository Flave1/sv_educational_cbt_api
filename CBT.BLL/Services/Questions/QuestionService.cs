using CBT.BLL.Constants;
using CBT.BLL.Services.Category;
using CBT.Contracts;
using CBT.Contracts.Authentication;
using CBT.Contracts.Candidates;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.Contracts.Questions;
using CBT.DAL;
using CBT.DAL.Models.Candidate;
using CBT.DAL.Models.Questions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.BLL.Services.Questions
{
    public class QuestionService : IQuestionService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _accessor;

        public QuestionService(DataContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
        }
        public async Task<APIResponse<CreateQuestion>> CreateQuestion(CreateQuestion request)
        {
            var res = new APIResponse<CreateQuestion>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());
                var userType = int.Parse(_accessor.HttpContext.Items["userType"].ToString());

                var examination = await _context.Examination.Where(m => m.ExaminationId == request.ExaminationId && m.ClientId == clientId).FirstOrDefaultAsync();
                if (examination == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "ExaminationId doesn't exist";
                    return res;
                }

                string options = "";
                string answers = "";

                foreach(string item in request.Options)
                {
                    if(item.Contains("</option>"))
                    {
                        options = $"{options}{item}";
                    }
                    else
                    {
                        options = $"{options}{item}</option>";
                    }
                    
                }

                foreach(int item in request.Answers)
                {
                   answers = $"{answers}{item},";
                }

                var question = new Question
                {
                    QuestionText = request.QuestionText,
                    ExaminationId = request.ExaminationId,
                    Mark = request.Mark,
                    Options = options,
                    Answers = answers,
                    QuestionType = request.QuestionType,
                    ClientId = clientId,
                    UserType = userType
                };

                _context.Question.Add(question);
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

        public async Task<APIResponse<IEnumerable<SelectQuestion>>> GetAllQuestions()
        {
            var res = new APIResponse<IEnumerable<SelectQuestion>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var questions = await _context.Question
                    .OrderByDescending(s => s.CreatedOn)
                    .Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Select(db => new SelectQuestion(db, _context.Examination.FirstOrDefault(x => x.ExaminationId == db.ExaminationId)))
                    .ToListAsync();

                res.IsSuccessful = true;
                res.Result = questions;
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

        public  async Task<APIResponse<SelectQuestion>> GetQuestion(Guid Id)
        {
            var res = new APIResponse<SelectQuestion>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var question = await _context.Question.Where(m => m.Deleted != true && m.QuestionId == Id && m.ClientId == clientId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var result = new SelectQuestion(question, _context.Examination.FirstOrDefault(x => x.ExaminationId == question.ExaminationId));

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

        public async Task<APIResponse<UpdateQuestion>> UpdateQuestion(UpdateQuestion request)
        {
            var res = new APIResponse<UpdateQuestion>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var question = await _context.Question.Where(m => m.QuestionId == request.QuestionId && m.ClientId == clientId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "QuestionId doesn't exist";
                    return res;
                }

                string options = "";
                string answers = "";

                foreach (string item in request.Options)
                {
                    if (item.Contains("</option>"))
                    {
                        options = $"{options}{item}";
                    }
                    else
                    {
                        options = $"{options}{item}</option>";
                    }

                }

                foreach (int item in request.Answers)
                {
                    answers = $"{answers}{item},";
                }

                question.QuestionText = request.QuestionText;
                question.ExaminationId = request.ExaminationId;
                question.Mark = request.Mark;
                question.Options = options;
                question.Answers = answers;
                question.QuestionType = request.QuestionType;

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
        public async Task<APIResponse<bool>> DeleteQuestion(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var question = await _context.Question.Where(d => d.Deleted != true && d.QuestionId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.Message.FriendlyMessage = "QuestionId does not exist";
                    res.IsSuccessful = true;
                    return res;
                }

                question.Deleted = true;
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

        public async Task<APIResponse<IEnumerable<SelectQuestion>>> GetQuestionByExamId(Guid examId)
        {
            var res = new APIResponse<IEnumerable<SelectQuestion>>();
            try
            {
                var clientId = Guid.Parse(_accessor.HttpContext.Items["userId"].ToString());

                var questions = await _context.Question
                     .OrderByDescending(s => s.CreatedOn)
                     .Where(d => d.Deleted != true && d.ExaminationId == examId && d.ClientId == clientId)
                     .Select(db => new SelectQuestion(db, _context.Examination.FirstOrDefault(x => x.ExaminationId == db.ExaminationId)))
                     .ToListAsync();

                res.IsSuccessful = true;
                res.Result = questions;
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
