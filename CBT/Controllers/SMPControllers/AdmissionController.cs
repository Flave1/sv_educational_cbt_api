using CBT.BLL.Middleware;
using CBT.BLL.Services.CandidateAnswers;
using CBT.BLL.Services.Candidates;
using CBT.BLL.Services.Result;
using CBT.Contracts.CandidateAnswers;
using CBT.Contracts.Candidates;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.AdmissionControllers
{
    [SmpAuthorize]
    [Route("cbt/api/v1/admission")]
    public class AdmissionController: Controller
    {
        private readonly ICandidateService candidateService;
        private readonly IResultService resultService;
        public AdmissionController(ICandidateService candidateService, IResultService resultService)
        {
            this.candidateService = candidateService;
            this.resultService = resultService;
        }
        [HttpPost("create-candidates")]
        public async Task<IActionResult> CreateCandidates([FromBody]CreateAdmissionCandidate request)
        {
            var response = await candidateService.CreateAdmissionCandidate(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-candidates-result")]
        public async Task<IActionResult> GetCandidateResult(string candidateCategoryId)
        {
            var response = await resultService.GetAdmissionCandidateResult(candidateCategoryId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
