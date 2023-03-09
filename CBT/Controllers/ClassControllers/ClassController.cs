using CBT.BLL.Middleware;
using CBT.BLL.Services.Class;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.ClassControllers
{
    [CbtAuthorize]
    [Route("cbt/api/v1/class")]
    public class ClassController: Controller
    {
        private readonly IClassService _service;

        public ClassController(IClassService service)
        {
            _service = service;
        }
        [HttpGet("get-active-classes")]
        public async Task<IActionResult> GetActiveClasses()
        {
            var response = await _service.GetActiveClasses();
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
