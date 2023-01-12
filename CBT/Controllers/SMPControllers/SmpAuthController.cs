using CBT.BLL.Filters;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Authentication;
using CBT.BLL.Services.Examinations;
using CBT.Contracts.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.AdmissionControllers
{
    //[SmpAuthorize]
    [Route("cbt/api/v1/smpauth")]
    public class SMPAuthController: Controller
    {
        private readonly IIdentityService service;


        public SMPAuthController(IIdentityService service)
        {
            this.service = service;
        }

        [HttpPost("login/by-hash")]
        public async Task<IActionResult> LoginByHash([FromBody] LoginCommandByHash req)
        {
            var response = await service.SMPLoginAsync(req);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
