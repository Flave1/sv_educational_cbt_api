using CBT.BLL.Middleware;
using CBT.BLL.Services.Settings;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using CBT.Contracts.Settings;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.SettingsController
{
    [CbtAuthorize]
    [Route("cbt/api/v1/settings")]
    public class SettingsController: Controller
    {
        private readonly ISettingService service;

        public SettingsController(ISettingService service)
        {
            this.service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCandidateCategory([FromBody] CreateSettings request)
        {
            var response = await service.CreateSettings(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-settings")]
        public async Task<IActionResult> GetAllCandidateCategory()
        {
            var response = await service.GetSettings();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCandidateCategory([FromBody] SingleDelete request)
        {
            var response = await service.DeleteSettings(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
