using CBT.BLL.Middleware;
using CBT.Contracts.Examination;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.QuestionControllers
{
    [CbtAuthorize]
    [Route("cbt/api/v1/question")]
    public class QuestionController : Controller
    {
        public QuestionController()
        {

        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuestion()
        {
            return Ok();
        }
    }
}
