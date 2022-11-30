using CBT.BLL.Constants;
using CBT.Contracts;
using CBT.Contracts.Common;
using CBT.Contracts.Questions;
using CBT.DAL;
using CBT.DAL.Models.Questions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CBT.BLL.Services.Questions
{
    public class QuestionService : IQuestionService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor accessor;

        public QuestionService(DataContext context, IHttpContextAccessor accessor)
        {
            this.context = context;
            this.accessor = accessor;
        }
        public async Task<APIResponse<CreateQuestion>> CreateQuestion(CreateQuestion request)
        {
            var res = new APIResponse<CreateQuestion>();
            try
            {
                var examination = await context.Examination.FirstOrDefaultAsync(x=>x.ExaminationId == Guid.Parse(request.ExaminationId));
                int? questionMarks = context.Question?.Where(x => x.ExaminationId == Guid.Parse(request.ExaminationId))?.Sum(x => x.Mark);

                if(questionMarks + request.Mark > examination.ExamScore)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "Questions mark must not exceed exam score!";
                    return res;
                }


                string options = String.Join("</option>", request.Options);
                string answers = String.Join(",", request.Answers);

                var question = new Question
                {
                    QuestionText = request.QuestionText,
                    ExaminationId = Guid.Parse(request.ExaminationId),
                    Mark = request.Mark,
                    Options = options,
                    Answers = answers,
                    QuestionType = request.QuestionType,
                };

                context.Question.Add(question);
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

        public async Task<APIResponse<IEnumerable<SelectQuestion>>> GetAllQuestions()
        {
            var res = new APIResponse<IEnumerable<SelectQuestion>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());
                var questions = await context.Question
                    .Where(d => d.Deleted != true && d.ClientId == clientId)
                    .Include(e=>e.Examination)
                    .OrderByDescending(s => s.CreatedOn)
                    .Select(db => new SelectQuestion(db))
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var question = await context.Question.Where(m => m.Deleted != true && m.QuestionId == Id && m.ClientId == clientId)
                    .Include(e=>e.Examination)
                    .FirstOrDefaultAsync();
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = Messages.FriendlyNOTFOUND;
                    return res;
                }

                var result = new SelectQuestion(question);
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
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var question = await context.Question.FirstOrDefaultAsync(m => m.QuestionId == request.QuestionId && m.ClientId == clientId);
                if (question == null)
                {
                    res.IsSuccessful = false;
                    res.Message.FriendlyMessage = "QuestionId doesn't exist";
                    return res;
                }

                string options = String.Join("</option>", request.Options);
                string answers = String.Join(",", request.Answers);

                question.QuestionText = request.QuestionText;
                question.ExaminationId = Guid.Parse(request.ExaminationId);
                question.Mark = request.Mark;
                question.Options = options;
                question.Answers = answers;
                question.QuestionType = request.QuestionType;

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
        public async Task<APIResponse<bool>> DeleteQuestion(SingleDelete request)
        {
            var res = new APIResponse<bool>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var question = await context.Question.Where(d => d.Deleted != true && d.QuestionId == Guid.Parse(request.Item) && d.ClientId == clientId).FirstOrDefaultAsync();
                if (question == null)
                {
                    res.Message.FriendlyMessage = "QuestionId does not exist";
                    res.IsSuccessful = false;
                    return res;
                }

                question.Deleted = true;
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

        public async Task<APIResponse<IEnumerable<SelectQuestion>>> GetQuestionByExamId(Guid examId)
        {
            var res = new APIResponse<IEnumerable<SelectQuestion>>();
            try
            {
                var clientId = Guid.Parse(accessor.HttpContext.Items["userId"].ToString());

                var questions = await context.Question
                     .Where(d => d.Deleted != true && d.ExaminationId == examId && d.ClientId == clientId)
                     .Include(e=>e.Examination)
                     .OrderByDescending(s => s.CreatedOn)
                     .Select(db => new SelectQuestion(db))
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
