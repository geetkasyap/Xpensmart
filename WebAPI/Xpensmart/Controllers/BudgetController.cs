using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BudgetController : Controller
    {
        public readonly BudgetService _budgetService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BudgetController> _budgetLogger;
        public BudgetController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<BudgetController> budgetsLogger)
        {
            _budgetService = new BudgetService(unitOfWork);
            _configuration = configuration;
            _budgetLogger = budgetsLogger;
        }
        // Endpoint to create a new Budget
        [HttpPost]
        [Route("AddBudget")]
        public async Task<GenericResponse> AddBudget([FromBody] BudgetsEntity budgetsEntities)
        {
            _budgetLogger.LogInformation("Calling Add budget");
            try
            {
                BudgetsEntity previousBudgets = _budgetService.CheckPreviousBudgets(budgetsEntities.StartDate, budgetsEntities.EndDate, budgetsEntities.UserID);
                if (previousBudgets != null) //check if there is any previous budget registered or not
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:BudgetAlreadyExists"] };
                }
                if (budgetsEntities == null) //if details are null
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:BudgetDataNull"] };
                }

                budgetsEntities.UsersEntity = null;
                BudgetsEntity expenseDetails = _budgetService.AddBudget(budgetsEntities);
                return new() { status = true, data = _configuration["GenericMessages:Values:AddBudgetSuccess"] };
            }
            catch (Exception ex)
            {
                _budgetLogger.LogError(ex, "Error adding budget");
                return new() { status = false, data = _configuration["GenericMessages:Values:AddBudgetFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to get budget entity by budget id
        [HttpGet]
        [Route("GetBudgetDetails/{BudgetID}")]
        public async Task<IActionResult> GetBudgetDetails(
            int BudgetID, [FromQuery] int UserID)
        {
            _budgetLogger.LogInformation("Calling Get BudgetDetails");
            try
            {
                if (BudgetID == 0)
                {
                    List<BudgetsEntity> budgetsEntities = await Task.FromResult(_budgetService.GetBudgetDetails());
                    return Ok(budgetsEntities);
                }
                else
                {
                    BudgetsEntity budgetsEntities = await Task.FromResult(_budgetService.GetBudgetDetails((int)BudgetID));
                    return Ok(budgetsEntities);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to update or alter budget entity by budget id
        [HttpPut]
        [Route("UpdateBudget/{BudgetID}")]
        public async Task<GenericResponse> UpdateBudget([FromRoute] int BudgetID, [FromQuery] decimal Amount)
        {
            _budgetLogger.LogInformation("Calling Update Budgets");
            try
            {
                BudgetsEntity budgetsEntities = await Task.FromResult(_budgetService.UpdateBudget(BudgetID, Amount));
                return new() { status = true, data = _configuration["GenericMessages:Values:UpdateBudgetDetailsSuccess"] };
            }
            catch (Exception ex)
            {
                _budgetLogger.LogError(ex, "Error updating budget");
                return new() { status = false, data = _configuration["GenericMessages:Values:UpdateBudgetDetailsFail"] };
            }
        }
        // Endpoint to delete budget by budget id 
        [HttpDelete]
        [Route("DeleteBudgets/{BudgetID}")]
        public async Task<GenericResponse> DeleteBudgets([FromRoute] int BudgetID)
        {
            _budgetLogger.LogInformation("Calling Delete Budgets");
            try
            {
                BudgetsEntity BudgetsEntity = await Task.FromResult(_budgetService.DeleteBudget(BudgetID));
                return new() { status = true, data = _configuration["GenericMessages:Values:DeleteBudgetsSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteBudgetsFail"] };
            }
        }
        // Endpoint to get a list of all the budgets by userid
        [HttpGet]
        [Route("GetBudgetsByUser")]
        public async Task<IActionResult> GetBudgetsByUser(int UserID)
        {
            _budgetLogger.LogInformation("Calling Get Budgets By User");
            try
            {
                List<BudgetsEntity> result = _budgetService.GetBudgetsByUser(UserID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

    }
}
