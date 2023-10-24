using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecurringTransactionsController : Controller
    {
        public readonly RecurringTransactionsService _transactionsService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RecurringTransactionsController> _transactionsLogger;
        public RecurringTransactionsController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<RecurringTransactionsController> transactionsLogger)
        {
            _transactionsService = new RecurringTransactionsService(unitOfWork);
            _configuration = configuration;
            _transactionsLogger = transactionsLogger;
        }
        // Endpoint to add recurring transactions
        [HttpPost]
        [Route("AddTransactions")]
        public async Task<GenericResponse> AddTransactions([FromQuery] DateTime FromDate, [FromQuery] DateTime ToDate, [FromQuery] string ExpenseType, [FromQuery] int CategoryID, [FromQuery] int UserID, [FromQuery] decimal Amount, [FromQuery] string RecieptUrl)
        {
            _transactionsLogger.LogInformation("Calling Add Transactions");  // Logging
            _transactionsLogger.LogInformation($"FromDate: {FromDate}, ToDate: {ToDate}, ExpenseType: {ExpenseType}, CategoryID: {CategoryID}, UserID: {UserID}, Amount: {Amount}, RecieptUrl: {RecieptUrl}");  // Logging
            try
            {
                ExpensesEntity transactionDetails = await _transactionsService.AddTransactionsAsync(FromDate,ToDate,ExpenseType,CategoryID,UserID,Amount, RecieptUrl);
                return new() { status = true, data = _configuration["GenericMessages:Values:AddTransactionsSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:AddTransactionsFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to get transaction details
        [HttpGet]
        [Route("GetTransactionDetails")]
        public async Task<IActionResult> GetTransactionDetails([FromQuery] DateTime date, [FromQuery] string ExpenseType, [FromQuery] int UserID)
        {
            _transactionsLogger.LogInformation("Calling Get TransactionsDetails"); // Logging
            try
            {
                if (ExpenseType == "All") // If all expense types are requested
                {
                    List<ExpensesEntity> transactionsEntities = await Task.FromResult(_transactionsService.GetTransactionsDetails((string)ExpenseType,(int)UserID));
                    return Ok(transactionsEntities);
                }
                else  // Fetching transactions for a specific date and type
                {
                    ExpensesEntity transactionsEntities = await Task.FromResult(_transactionsService.GetTransactionsDetails((DateTime)date, (string)ExpenseType, (int) UserID));
                    return Ok(transactionsEntities);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        // Endpoint to update a transaction
        [HttpPut]
        [Route("UpdateTransactions/{ExpenseID}")]
        public async Task<GenericResponse> UpdateTransaction([FromRoute] int ExpenseID, [FromBody] ExpensesEntity updatedExpense, [FromQuery] int UserID)
        {
            _transactionsLogger.LogInformation("Calling Update Transactions"); // Logging 
            try
            {
                if (ExpenseID != updatedExpense.ExpenseID) // Checking for ID mismatch
                {
                    return new() { status = true, data = _configuration["GenericMessages:Values:ExpenseIDMismatch"] };
                }
                // Updating the transaction using the service
                ExpensesEntity transactionsEntities = await Task.FromResult(_transactionsService.UpdateTransactions(ExpenseID, UserID, updatedExpense));

                return new() { status = true, data = _configuration["GenericMessages:Values:UpdateTransactionsDetailsSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:UpdateTransactionsDetailsFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to delete a transaction
        [HttpDelete]
        [Route("DeleteTransactions/{ExpenseID}")]
        public async Task<GenericResponse> DeleteTransactions([FromRoute] int ExpenseID)
        {
            _transactionsLogger.LogInformation("Calling Delete Transactions"); // Logging 
            try
            {
                ExpensesEntity ExpensesEntity = await Task.FromResult(_transactionsService.DeleteTransactions(ExpenseID));
                return new() { status = true, data = _configuration["GenericMessages:Values:DeleteTransactionsSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteTransactionsFail"], error = new Error() { description = ex.Message } };
            }

        }


    }
}
