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
        public async Task<IActionResult> GetAllCandidateResult(string examinationId)
        {
            var response = await service.GetAllCandidateResult(examinationId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
