using CBT.BLL.Middleware;
using CBT.BLL.Services.WebRequests;
using CBT.Contracts;
using CBT.Contracts.Options;
using CBT.Contracts.Routes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Web.Http.Controllers;

namespace CBT.Controllers.Examination
{

    [CbtAuthorize]
    [Route("cbt/api/v1/examination")]
    public class ExaminationController : Controller
    {
        private readonly IWebRequest _service;
        private readonly FwsConfigSettings _fwsOptions;

        public ExaminationController(IWebRequest service, IOptions<FwsConfigSettings> fwsOptions)
        {
            _service = service;
            _fwsOptions = fwsOptions.Value;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateExamination()
        {
            try
            {

                //string userId = HttpContext.Items["userId"].ToString();
                //return Ok(await _service.GetAsync<APIResponse<List<SelectLookup>>>($"{_fwsOptions.FwsBaseUrl}{FwsRoutes.countrySelect}"));
                return Ok();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }


}
