using CBT.BLL.Filters;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Examinations;
using CBT.BLL.Services.Result;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.AdmissionControllers
{
    [SmpAuthorize]
    [Route("cbt/api/v1/smpexams")]
    public class SMPExamsController: Controller
    {
        private readonly IExaminationService examService;
        private readonly IResultService resultService;

        public SMPExamsController(IExaminationService examService, IResultService service)
        {
            this.examService = examService;
            this.resultService = service;
        }

        [HttpGet("get-all-examination/by-sessionclass")]
        public async Task<IActionResult> GetAllExamination2(PaginationFilter filter, string sessionClassId)
        {
            var response = await examService.GetAllExamination2(filter, sessionClassId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("get-result")]
        public async Task<IActionResult> GetResult()
        {
            var response = await resultService.GetCandidateResult();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
