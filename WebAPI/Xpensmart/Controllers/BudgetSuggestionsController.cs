using ApplicationLayer.Interfaces;
using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BudgetSuggestionsController : Controller
    {
        private readonly BudgetSuggestionsService _budgetSuggestionsService;
        public BudgetSuggestionsController(IUnitOfWork unitOfWork)
        {
            _budgetSuggestionsService = new BudgetSuggestionsService(unitOfWork);
        }
        // Endpoint to get all suggestion notifications after calculation
        [HttpGet("GetSuggestions/{userId}")]
        public async Task<IActionResult> GetBudgetSuggestions(int userId)
        {
            try
            {
                var suggestions = await _budgetSuggestionsService.GetBudgetSuggestionsAsync(userId);
                if (suggestions == null || suggestions.Count == 0)
                {

                    return Ok(new { status = "Failure", data = "Insufficient data for suggestions." });
                }
                return Ok(new { status = true, data = suggestions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
