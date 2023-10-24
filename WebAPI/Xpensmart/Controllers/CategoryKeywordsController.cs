using ApplicationLayer.Interfaces;
using BusinessLayer;
using Microsoft.AspNetCore.Mvc;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryKeywordsController : Controller
    {
        public readonly CategoryKeywordsService _categoryKeywordsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoryKeywordsController> _categoryKeywordsLogger;
        public CategoryKeywordsController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<CategoryKeywordsController> categoryKeywordsLogger)
        {
            _categoryKeywordsService = new CategoryKeywordsService(unitOfWork);
            _configuration = configuration;
            _categoryKeywordsLogger = categoryKeywordsLogger;
        }
        // Endpoint to get all category keywords for smart cateogry selection
        [HttpGet]
        [Route("GetCategoryKeywords")]
        public IActionResult GetCategoryKeywords()
        {
            try
            {
                var categoryKeywords = _categoryKeywordsService.GetCategoryKeywords();
                return Ok(categoryKeywords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            
        }
    }
}
