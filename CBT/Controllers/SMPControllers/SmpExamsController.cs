using CBT.BLL.Filters;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Examinations;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.AdmissionControllers
{
    [SmpAuthorize]
    [Route("cbt/api/v1/smpexams")]
    public class SMPExamsController: Controller
    {
        private readonly IExaminationService _examService;

        public SMPExamsController(IExaminationService examService)
        {
            _examService = examService;
        }

        [HttpGet("get-all-examination/by-sessionclass")]
        public async Task<IActionResult> GetAllExamination2(PaginationFilter filter, string sessionClassId)
        {
            var response = await _examService.GetAllExamination2(filter, sessionClassId);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
