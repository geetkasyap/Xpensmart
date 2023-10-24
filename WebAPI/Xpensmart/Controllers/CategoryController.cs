using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Xpensmart.Controllers
{
    public class CategoryController : Controller
    {
        public readonly CategoryService categoryService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoryController> _categoryLogger;
        public CategoryController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<CategoryController> categoryLogger)
        {
            categoryService = new CategoryService(unitOfWork);
            _configuration = configuration;
            _categoryLogger = categoryLogger;
        }
        // Endpoint to get all the 12 category entities
        [HttpGet]
        [Route("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            _categoryLogger.LogInformation("Calling Get Categories");
            try
            {
                    List<CategoriesEntity> categoriesEntity = await Task.FromResult(categoryService.GetCategories());
                    return Ok(categoriesEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
