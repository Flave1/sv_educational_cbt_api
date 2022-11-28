using CBT.BLL.Middleware;
using CBT.BLL.Services.CandidateAnswers;
using CBT.BLL.Services.Candidates;
using CBT.Contracts.CandidateAnswers;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.CandidateControllers
{
    [CbtAuthorize]
    [Route("cbt/api/v1/candidate-answer")]
    public class CandidateAnswerController: Controller
    {
        private readonly ICandidateAnswerService _service;

        public CandidateAnswerController(ICandidateAnswerService service)
        {
            _service = service;
        }
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitCandidateAnswer([FromBody] CreateCandidateAnswer request)
        {
            var response = await _service.SubmitCandidateAnswer(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("submit-all")]
        public async Task<IActionResult> SubmitAllCandidateAnswer([FromBody] SubmitAllAnswers request)
        {
            var response = await _service.SubmitAllCandidateAnswer(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-candidate-answers")]
        public async Task<IActionResult> GetAllCandidatesAnswer()
        {
            var response = await _service.GetAllCandidateAnswers();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-candidate-answer/{id}")]
        public async Task<IActionResult> GetCandidateAnswer(string id)
        {
            var response = await _service.GetCandidateAnswer(Guid.Parse(id));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCandidateAnswer([FromBody]UpdateCandidateAnswer request)
        {
            var response = await _service.UpdateCandidateAnswer(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCandidateAnswer([FromBody]SingleDelete request)
        {
            var response = await _service.DeleteCandidateAnswer(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
