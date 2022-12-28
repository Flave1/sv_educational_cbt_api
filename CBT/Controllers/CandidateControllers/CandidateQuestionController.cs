using CBT.BLL.Filters;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Questions;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.CandidateControllers
{
    [CandidateAuthorize]
    [Route("cbt/api/v1/candidate-questions")]
    public class CandidateQuestionController: Controller
    {
        private readonly IQuestionService service;

        public CandidateQuestionController(IQuestionService service)
        {
            this.service = service;
        }
        [HttpGet("get-all-questions")]
        public async Task<IActionResult> GetCandidateQuestions(PaginationFilter filter)
        {
            var response = await service.GetCandidateQuestions(filter);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-question/{questionId}")]
        public async Task<IActionResult> GetCandidateQuestions(string questionId)
        {
            var response = await service.GetCandidateQuestionById(questionId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
