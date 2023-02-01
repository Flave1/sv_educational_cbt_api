using CBT.BLL.Filters;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Result;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.ResultController
{
    [CbtAuthorize]
    [Route("cbt/api/v1/examiner-result")]
    public class ExaminerResultController: Controller
    {
        private readonly IResultService service;

        public ExaminerResultController(IResultService service)
        {
            this.service = service;
        }

        [HttpGet("get-all-candidate-result")]
        public async Task<IActionResult> GetAllCandidateResult(PaginationFilter filter, string examinationId)
        {
            var response = await service.GetAllCandidateResult(filter, examinationId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("download-candidate-result")]
        public async Task<IActionResult> DownloadCandidateResult(string examinationId)
        {
            var response = await service.DownloadCandidateResult(examinationId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-candidate-answers")]
        public async Task<IActionResult> GetCandidateAnswers(PaginationFilter filter, string examinationId, string candidateId, string candidateEmail)
        {
            var response = await service.GetCandidateAnswers(filter, examinationId, candidateId, candidateEmail);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("reset-result")]
        public async Task<IActionResult> ResetResult(string examinationId, string candidateId_regNo)
        {
            var response = await service.ResetResult(examinationId, candidateId_regNo);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
