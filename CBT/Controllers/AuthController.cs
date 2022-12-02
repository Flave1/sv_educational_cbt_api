using CBT.Contracts.Routes;
using CBT.Contracts;
using Microsoft.AspNetCore.Mvc;
using CBT.BLL.Services.Authentication;
using CBT.Contracts.Authentication;

namespace CBT.Controllers
{
    [Route("cbt/api/v1")]
    public class AuthController: Controller
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand user)
        {
            var response = await _identityService.LoginAsync(user);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
