using CBT.BLL.Middleware;
using CBT.BLL.Services.Examinations;
using CBT.Contracts.Common;
using CBT.Contracts.Examinations;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.Examination
{

    [CbtAuthorize]
    [Route("cbt/api/v1/examination")]
    public class ExaminationController : Controller
    {
        private readonly IExaminationService _service;

        public ExaminationController(IExaminationService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateExamination([FromBody]CreateExamination request)
        {
            var response = await _service.CreateExamination(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-examination")]
        public async Task<IActionResult> GetAllExamination()
        {
            var response = await _service.GetAllExamination();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-single-examination/{id}")]
        public async Task<IActionResult> GetExamination(string id)
        {
            var response = await _service.GetExamination(Guid.Parse(id));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateExamination([FromBody]UpdateExamination request)
        {
            var response = await _service.UpdateExamination(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteExamination([FromBody]SingleDelete request)
        {
            var response = await _service.DeleteExamination(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }


}
