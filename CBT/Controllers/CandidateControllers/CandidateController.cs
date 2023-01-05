using CBT.BLL.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CBT.BLL.Services.Candidates;
using CBT.Contracts.Candidates;
using CBT.Contracts.Common;
using CBT.BLL.Filters;

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
        public async Task<IActionResult> GetAllCandidates(PaginationFilter filter)
        {
            var response = await _service.GetAllCandidates(filter);
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
        [HttpPost("create-admission-candidate")]
        public async Task<IActionResult> CreateAdmissionCandidate([FromBody]CreateAdmissionCandidate request)
        {
            var response = await _service.CreateAdmissionCandidate(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpPost("login/examId")]
        public async Task<IActionResult> LoginByExamId([FromBody]CandidateLoginExamId request)
        {
            var response = await _service.LoginByExamId(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpPost("login/email")]
        public async Task<IActionResult> LoginByEmail([FromBody] CandidateLoginEmail request)
        {
            var response = await _service.LoginByEmail(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [AllowAnonymous]
        [HttpPost("login/reg-no")]
        public async Task<IActionResult> LoginByRegNo([FromBody] CandidateLoginRegNo request)
        {
            var response = await _service.LoginByRegNo(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
