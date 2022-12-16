using CBT.BLL.Filters;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Dashboard;
using CBT.BLL.Services.Examinations;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.ExaminationControllers
{
    [CbtAuthorize]
    [Route("cbt/api/v1/dashboard")]
    public class DashboardController: Controller
    {
        private readonly IDashboardService service;

        public DashboardController(IDashboardService service)
        {
            this.service = service;
        }
        [HttpGet("examiner-dashboard-count")]
        public async Task<IActionResult> GetExaminerDashboardCount()
        {
            var response = await service.GetExaminerDashboardCount();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
