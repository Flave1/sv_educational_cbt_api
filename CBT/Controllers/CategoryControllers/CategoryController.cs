using CBT.BLL.Middleware;
using CBT.BLL.Services.Category;
using CBT.Contracts.Category;
using CBT.Contracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace CBT.Controllers.Category
{

    [CbtAuthorize]
    [Route("cbt/api/v1/category")]
    public class CategoryController: Controller
    {
        private readonly ICandidateCategoryService _service;
        public CategoryController(ICandidateCategoryService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCandidateCategory([FromBody] CreateCandidateCategory request)
        {
            var response = await _service.CreateCandidateCategory(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-all-categories")]
        public async Task<IActionResult> GetAllCandidateCategory()
        {
            var response = await _service.GetAllCandidateCategory();
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpGet("get-single-category/{id}")]
        public async Task<IActionResult> GetCandidateCategory(string id)
        {
            var response = await _service.GetCandidateCategory(Guid.Parse(id));
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("update")]
        public async Task<IActionResult> UpdateCandidateCategory([FromBody]UpdateCandidateCategory request)
        {
            var response = await _service.UpdateCandidateCategory(request);
            if(response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteCandidateCategory([FromBody]SingleDelete request)
        {
            var response = await _service.DeleteCandidateCategory(request);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
