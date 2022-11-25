using CBT.BLL.Middleware;
using CBT.BLL.Services.Questions;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.Contracts.Questions;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.QuestionControllers
{
    [CbtAuthorize]
    [Route("cbt/api/v1/question")]
    public class QuestionController : Controller
    {
        private readonly IQuestionService _service;

        public QuestionController(IQuestionService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuestion([FromBody]CreateQuestion request)
        {
            var response = await _service.CreateQuestion(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-questions")]
        public async Task<IActionResult> GetAllQuestions()
        {
            var response = await _service.GetAllQuestions();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-single-questions/{id}")]
        public async Task<IActionResult> GetQuestion(string id)
        {
            var response = await _service.GetQuestion(Guid.Parse(id));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-all-questions/exam/{examId}")]
        public async Task<IActionResult> GetQuestionByExamId(string examId)
        {
            var response = await _service.GetQuestionByExamId(Guid.Parse(examId));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateGetQuestion([FromBody] UpdateQuestion request)
        {
            var response = await _service.UpdateQuestion(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteExamination([FromBody] SingleDelete request)
        {
            var response = await _service.DeleteQuestion(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
