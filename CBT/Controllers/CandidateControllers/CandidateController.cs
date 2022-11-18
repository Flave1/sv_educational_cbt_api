using CBT.Contracts.Routes;
using CBT.Contracts;
using Microsoft.AspNetCore.Mvc;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Category;
using CBT.Contracts.Category;
using CBT.BLL.Services.Candidates;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;

namespace CBT.Controllers.CandidateController
{
    [CbtAuthorize]
    [Route("cbt/api/v1/candidate")]
    public class CandidateController: Controller
    {
        private readonly ICandidateService _service;

        public CandidateController(ICandidateService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCandidate([FromForm]CreateCandidate request)
        {
            var response = await _service.CreateCandidate(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-candidates")]
        public async Task<IActionResult> GetAllCandidates()
        {
            var response = await _service.GetAllCandidates();
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-candidate/{id}")]
        public async Task<IActionResult> GetCandidate(string id)
        {
            var response = await _service.GetCandidate(id);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCandidate([FromForm]UpdateCandidate request)
        {
            var response = await _service.UpdateCandidate(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCandidate([FromBody] SingleDelete request)
        {
            var response = await _service.DeleteCandidate(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
