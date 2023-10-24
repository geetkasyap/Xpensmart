using Microsoft.AspNetCore.Mvc;
using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using System.Security.Claims;
using CoreModels.DTO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExpensesController : Controller
    {
        public readonly ExpensesService _expensesService;
        public readonly CategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExpensesController> _expensesLogger;
        public ExpensesController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<ExpensesController> expensesLogger)
        {
            _expensesService = new ExpensesService(unitOfWork);
            _categoryService = new CategoryService(unitOfWork);
            _configuration = configuration;
            _expensesLogger = expensesLogger;
        }
        // Endpoint to add new expenses 
        [HttpPost]
        [Route("AddExpenses")]
        public async Task<GenericResponse> AddExpenses([FromBody] ExpensesEntity expensesEntities)
        {
            _expensesLogger.LogInformation("Calling Add Expenses");
            try
            {
                if (expensesEntities == null)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:ExpenseDataNull"] };
                } 
                //setting entites as null
                expensesEntities.UsersEntity = null;
                expensesEntities.CategoriesEntity = null;

                ExpensesEntity ExepenseDetails =await _expensesService.AddExpensesAsync(expensesEntities);
                return new() { status = true, data = _configuration["GenericMessages:Values:AddExpensesSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:AddExpensesFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to get expense entity by enpense id
        [HttpGet]
        [Route("GetExpenseDetails/{ExpenseID}")]
        public async Task<IActionResult> GetExpenseDetails([FromRoute] int ExpenseID, [FromQuery] int UserID)
        {
            _expensesLogger.LogInformation("Calling Get ExpensesDetails");
            try
            {
                if (ExpenseID == 0 && UserID==0) //returns a list of all expnese entities of all the users
                {
                    List<ExpensesEntity> expensesEntities = await Task.FromResult(_expensesService.GetExpensesDetails());
                    return Ok(expensesEntities);
                }
                if(ExpenseID == 0 && UserID != 0) //returns all expnese entities for a particular userid
                {
                    List<ExpensesEntity> expensesEntities = await Task.FromResult(_expensesService.GetAllExpensesDetails((int)UserID));
                    return Ok(expensesEntities);
                }
                else //returns an entity of a expense entity by expense id
                {
                    ExpensesEntity expensesEntities = await Task.FromResult(_expensesService.GetExpensesDetails((int)ExpenseID));
                    return Ok(expensesEntities);
                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        // Endpoint to update expense entity by expense id 
        [HttpPut]
        [Route("UpdateExpenses/{ExpenseID}")]
        public async Task<GenericResponse> UpdateExpenses([FromRoute] int ExpenseID, [FromBody] ExpensesEntity updatedExpense, [FromQuery] int UserID)
        {
            _expensesLogger.LogInformation("Calling Update Expenses");
            try
            {
                if (ExpenseID != updatedExpense.ExpenseID)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:UpdateExpenseMismatch"] };
                }

                ExpensesEntity expensesEntities = await Task.FromResult(_expensesService.UpdateExpenses(ExpenseID, UserID, updatedExpense));
                return new() { status = true, data = _configuration["GenericMessages:Values:UpdateExpensesDetailsSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:UpdateExpensesDetailsFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to delete expense entity by expense id 
        [HttpDelete]
        [Route("DeleteExpenses/{ExpenseID}")]
        public async Task<GenericResponse> DeleteExpenses([FromRoute] int ExpenseID)
        {
            _expensesLogger.LogInformation("Calling Delete Expenses");
            try
            {
                await Task.FromResult(_expensesService.DeleteExpenses(ExpenseID));
                return new() { status = true, data = _configuration["GenericMessages:Values:DeleteExpensesSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteExpensesFail"], error = new Error() { description = ex.Message } };
            }

        }
        // Endpoint to generate report between dates of specific category id
        [HttpGet]
        [Route("GenerateReport")]
        public IActionResult GenerateReport([FromQuery] int UserID,[FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int CategoryID)
        {
            _expensesLogger.LogInformation("Calling Generate Report");
            try
            {
                var result = _expensesService.GenerateReport(UserID, startDate, endDate, CategoryID);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to get upcoming type expense entity list
        [HttpGet]
        [Route("GetUpcomingExpenses/{userId}")]
        public IActionResult GetUpcomingExpenses(int userId)
        {
            _expensesLogger.LogInformation("Calling Get Upcoming Expenses");
            try
            {
                var result = _expensesService.GetUpcomingExpenses(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to convert currency 
        [HttpGet]
        [Route("ConvertCurrency")]
        public IActionResult ConvertCurrency([FromQuery] decimal amount, [FromQuery] string fromCurrency, [FromQuery] string toCurrency)
        {
            _expensesLogger.LogInformation("Calling Convert Currency");
            try
            {
                var result = _expensesService.ConvertCurrency(amount, fromCurrency, toCurrency);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to add expenses by bulk upload
        [HttpPost("bulk-import/{UserID}")]
        public async Task<GenericResponse> BulkImport([FromRoute] int UserID, IFormFile file)
        {
            if (file == null || file.Length == 0) //if file is empty
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:NoFileUpload"] };
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream())) //Opens the request stream for reading the uploaded file.
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)  //CsvHelper requires you to specify the CultureInfo the culture which is used to determine the default delimiter, default line ending, and formatting when type converting.InvariantCulture will be the most portable for writing a file and reading it back
                {
                    HeaderValidated = null, 
                    MissingFieldFound = null  
                }))
                {
                    csv.Read();
                    csv.ReadHeader();

                    var headers = csv.HeaderRecord; //check headers
                    if (!headers.Contains("CategoryName") || !headers.Contains("Amount") || !headers.Contains("Date")) //header names specified
                    {
                        return new() { status = false, data = _configuration["GenericMessages:Values:InvalidHeaders"] };
                    }

                    var records = new List<ExpensesCsvModel>();
                    while (csv.Read())
                    {
                        var record = new ExpensesCsvModel 
                        {
                            CategoryName = csv.GetField<string>("CategoryName"), //retrieves the value of the "CategoryName" field from the CSV
                            Amount = csv.GetField<decimal?>("Amount") ?? 0, //retrieves the value of the "Amount" field, which is expected to be a nullable decimal (`decimal?`). If the value is `null`, it defaults to `0`
                            Date = csv.GetField<DateTime?>("Date") ?? DateTime.Now, //retrieves the value of the "Date" field, which is expected to be a nullable `DateTime`. If the value is `null`, it defaults to the current date and time 
                            ReceiptUrl = csv.GetField<string>("ReceiptUrl"), //retrieves the value of the "ReceiptUrl" field and assigns it to the `ReceiptUrl` property
                            Description = csv.GetField<string>("Description"), //retrieves the value of the "Description" field and assigns it to the `Description` property
                            ExpenseType = csv.GetField<string>("ExpenseType") //retrieves the value of the "ExpenseType" field and assigns it to the `ExpenseType` property
                        };
                        var category = await _categoryService.GetCategoryByNameAsync(record.CategoryName);
                        if (category == null)
                        {
                            return new() { status = false, data = $"Category '{record.CategoryName}' not found." };
                        }

                        records.Add(record);
                    }

                    foreach (var record in records)
                    {
                        var category = await _categoryService.GetCategoryByNameAsync(record.CategoryName); //add to expense entity and add it by async
                        await _expensesService.AddBulkExpensesAsync(UserID, new ExpensesEntity
                        {
                            UserID = UserID,
                            Amount = record.Amount,
                            Date = record.Date,
                            CategoryID = category.CategoryID,
                            ReceiptUrl = record.ReceiptUrl,
                            Description = record.Description,
                            ExpenseType = record.ExpenseType,
                            IsActive = true,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            CreatedBy = "system",
                            ModifiedBy = "system"
                        });
                    }
                }

                return new() { status = true, data = _configuration["GenericMessages:Values:BulkExpenseSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:BulkExpenseFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to split expenses to all users
        [HttpPost("split-expense")]
        public async Task<GenericResponse> SplitExpense([FromQuery] int userId, [FromQuery] int categoryID, [FromQuery] string FileUrl, [FromBody] ExpenseSplitRequest splitRequest )
        {
            _expensesLogger.LogInformation("Calling Split Expense");
            try
            {
                if (splitRequest == null || splitRequest.UserNames == null || splitRequest.UserNames.Count == 0) // if request was null 
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:SplitExpenseNull"] };
                } 

                var result = await _expensesService.SplitExpenseAsync(splitRequest, userId, categoryID, FileUrl);
                return new() { status = true, data = _configuration["GenericMessages:Values:SplitExpenseSuccess"] };
            }
            catch (Exception ex)
            {
                _expensesLogger.LogError($"Error splitting expense: {ex.Message}");

                return new() { status = false, data = _configuration["GenericMessages:Values:SplitExpenseFail"], error = new Error() { description = ex.Message } };
            }   
        }
    }
}
