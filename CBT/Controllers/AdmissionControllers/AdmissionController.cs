using CBT.BLL.Middleware;
using CBT.BLL.Services.CandidateAnswers;
using CBT.BLL.Services.Candidates;
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
        public AdmissionController(ICandidateService candidateService)
        {
            this.candidateService = candidateService;
        }
        [HttpPost("create-candidates")]
        public async Task<IActionResult> CreateCandidates([FromBody]CreateAdmissionCandidate request)
        {
            var response = await candidateService.CreateAdmissionCandidate(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
