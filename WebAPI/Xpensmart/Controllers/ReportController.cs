using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels.DTO;
using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        public readonly ReportService _reportService;
        public readonly CategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ReportController> _reportLogger;
        public ReportController(IUnitOfWork unitOfWork,IAggregateBudgetRepository aggregateBudgetRepository,IConfiguration configuration,ILogger<ReportController> reportLogger)
        {
            _reportService = new ReportService(unitOfWork, aggregateBudgetRepository);
            _configuration = configuration;
            _reportLogger = reportLogger;
            _categoryService = new CategoryService(unitOfWork);
        }

        // Endpoint to generate a total expense report
        [HttpGet]
        [Route("GenerateTotalExpenseReport")]
        public IActionResult GenerateTotalExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            try
            {
                // Calling the service to generate the total expense report
                decimal reportAmount = _reportService.GenerateTotalExpenseReport(userId, startDate, endDate, categoryId);

                return Ok(new
                {
                    CategoryId = categoryId,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalAmount = reportAmount
                });
            }
            catch (Exception ex)
            {
                // Handling exceptions and returning a bad request response
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to generate an average expense report
        [HttpGet]
        [Route("GenerateAverageExpenseReport")]
        public IActionResult GenerateAverageExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            try
            {
                decimal reportAmount = _reportService.GenerateAverageExpenseReport(userId, startDate, endDate, categoryId);

                return Ok(new
                {
                    CategoryId = categoryId,
                    StartDate = startDate,
                    EndDate = endDate,
                    AverageAmount = reportAmount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get the top three recent expenses for a user
        [HttpGet("top-three-recent-expenses")]
        public IActionResult GetTopThreeRecentExpenses(int userId)
        {
            _reportLogger.LogInformation("Calling Get Top Three Recent Expenses");

            try
            {
                var recentExpenses = _reportService.GetTopThreeRecentExpenses(userId);
                return Ok(recentExpenses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }

        // Endpoint to get upcoming expenses for a user
        [HttpGet("upcoming-expenses")]
        public IActionResult GetUpcomingExpenses(int userId)
        {
            _reportLogger.LogInformation("Calling  Get Upcoming Expenses");

            try
            {
                var upcomingExpenses = _reportService.GetUpcomingExpenses(userId);
                return Ok(upcomingExpenses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get remaining expenses for a user
        [HttpGet("remaining-expenses")]
        public IActionResult GetRemainingExpenses(int userId)
        {
            _reportLogger.LogInformation("Calling Get Remaining Expenses");

            try
            {
                var remainingExpenses = _reportService.GetRemainingExpenses(userId);
                return Ok(remainingExpenses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get expenses by date range for a user
        [HttpGet]
        [Route("GetExpensesByDates")]
        public IActionResult GetExpensesByDates([FromQuery] int userId, [FromQuery]  DateTime startDate, [FromQuery] DateTime endDate)
        {
            _reportLogger.LogInformation("Calling Get ExpensesByDates");
            try
            {
                List<ExpensesEntity> expensesEntities =_reportService.GetExpensesByDates(userId, startDate, endDate);

                return Ok(expensesEntities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get budget by date range for a user
        [HttpGet]
        [Route("GetBudgetByDates")]
        public IActionResult GetBudgetByDates([FromQuery] int userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            _reportLogger.LogInformation("Calling Get BudgetByDates");
            try
            {
                decimal budgetEntities = _reportService.GetBudgetByDates(userId, startDate, endDate);

                return Ok(budgetEntities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get category expenses by date range
        [HttpGet]
        [Route("GetCategoryExpensesByDates")]
        public IActionResult GetCategoryExpensesByDates([FromQuery] int userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int CategoryID)
        {
            _reportLogger.LogInformation("Calling Get CategoryExpensesByDates");
            try
            {
                List<ExpensesEntity> expensesEntities = _reportService.GetCategoryExpensesByDates(userId, startDate, endDate,CategoryID);

                return Ok(expensesEntities);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get a summary of expenses by category
        [HttpGet]
        [Route("GetCategorySummary")]
        public IActionResult GetCategorySummary([FromQuery] int userId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            _reportLogger.LogInformation("Calling Get Category Summary");
            try
            {
                var summary = _reportService.GetCategorySummary(userId, startDate, endDate);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get category budgets within a date range
        [HttpGet]
        [Route("expenses/category-budgets")]
        public IActionResult GetCategoryBudgets([FromQuery] int userId, [FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] int categoryid)
        {
            _reportLogger.LogInformation("Calling Get Category Budgets");
            try
            {
                var budgets = _reportService.GetCategoryBudgets(userId, start, end,categoryid);
                return Ok(budgets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get monthly budgets within a date range
        [HttpGet]
        [Route("GetMonthlyBudgets")]
        public IActionResult GetMonthlyBudgets([FromQuery] int userId,[FromQuery] DateTime startDate,[FromQuery] DateTime endDate)
        {
            _reportLogger.LogInformation("Calling Get Monthly Budgets");
            try
            {
                var monthlyBudgets = _reportService.GetMonthlyBudgetsByDateRange(userId, startDate, endDate);
                return Ok(monthlyBudgets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }

        // Endpoint to project future expenses based on user input
        [HttpPost("project-expenses")]
        public async Task<IActionResult> ProjectExpenses([FromBody] ExpenseProjectionRequest request)
        {
            _reportLogger.LogInformation("Calling Project Expenses");

            try
            {
                var projection = await _reportService.CalculateExpenseProjectionAsync(request.UserId, request.Income);
                return Ok(projection);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
        // Endpoint to get expenses for the present month
        [HttpGet("present-month-expenses")]
        public IActionResult GetPresentMonthExpenses([FromQuery] int userId)
        {
            _reportLogger.LogInformation("Calling Get Present Month Expenses");

            try
            {
                var currentDate = DateTime.Now;
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var categories = _categoryService.GetCategories();
                var presentMonthExpenses = categories.ToDictionary(
                    c => c.CategoryID,
                    c => _reportService.GetCategoryExpensesByDates(userId, firstDayOfMonth, lastDayOfMonth, c.CategoryID)
                        .Sum(e => e.Amount)
                );

                return Ok(presentMonthExpenses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { data = ex.Message });
            }
        }
    }
}
