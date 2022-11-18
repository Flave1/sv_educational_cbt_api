using CBT.BLL.Middleware;
using CBT.BLL.Services.Subject;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.SubjectControllers
{
    [CbtAuthorize]
    [Route("cbt/api/v1/subject")]
    public class SubjectController: Controller
    {
        private readonly ISubjectService _service;

        public SubjectController(ISubjectService service)
        {
            _service = service;
        }
        [HttpGet("get-active-subjects")]
        public async Task<IActionResult> GetActiveSubjects()
        {
            var response = await _service.GetActiveSubjects();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
