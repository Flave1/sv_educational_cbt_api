using CBT.Contracts.Routes;
using CBT.Contracts;
using Microsoft.AspNetCore.Mvc;
using CBT.BLL.Middleware;
using CBT.BLL.Services.Candidate;
using CBT.Contracts.Candidate;

namespace CBT.Controllers.CandidateController
{
    [CbtAuthorize]
    [Route("cbt/api/v1/candidate-category")]
    public class CandidateController: Controller
    {
        private readonly ICandidateCategoryService _categoryService;

        public CandidateController(ICandidateCategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCandidateCategory([FromBody]CreateCandidateCategory request)
        {
            var response =  await _categoryService.CreateCandidateCategory(request, Guid.Parse(HttpContext.Items["userId"].ToString()), int.Parse(HttpContext.Items["userType"].ToString()));
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-categories")]
        public async Task<IActionResult> GetAllCandidateCategory()
        {
            var response = await _categoryService.GetAllCandidateCategory();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-category/{id}")]
        public async Task<IActionResult> GetAllCandidateCategory(string id)
        {
            var response = await _categoryService.GetCandidateCategory(Guid.Parse(id));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCandidateCategory()
        {
            return Ok();
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCandidateCategory()
        {
            return Ok();
        }

    }
}
