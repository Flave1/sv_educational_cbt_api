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
    }
}
