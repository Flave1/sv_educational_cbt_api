using CBT.BLL.Middleware;
using CBT.BLL.Services.Result;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.ResultController
{
    [CandidateAuthorize]
    [Route("cbt/api/v1/result")]
    public class ResultController: Controller
    {
        private readonly IResultService service;

        public ResultController(IResultService service)
        {
            this.service = service;
        }
        [HttpGet("get-result")]
        public async Task<IActionResult> GetResult()
        {
            var response = await service.GetResult();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

    }
}
