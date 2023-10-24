using ApplicationLayer.Interfaces;
using BusinessLayer;
using CoreModels;
using CoreModels.DTO;
using CoreModels.Entities;
using DataAccessLayer.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Linq;

namespace Xpensmart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupsController : Controller
    {
        public readonly UsersService _usersService;
        public readonly GroupsService _groupsService;
        public readonly ExpensesService _expensesService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GroupsController> _groupsLogger;
        public GroupsController(IUnitOfWork unitOfWork, IConfiguration configuration, ILogger<GroupsController> groupsLogger)
        {
            _usersService = new UsersService(unitOfWork);
            _groupsService = new GroupsService(unitOfWork);
            _expensesService = new ExpensesService(unitOfWork);
            _configuration = configuration;
            _groupsLogger = groupsLogger;
        }
        // Endpoint to create a new group
        [HttpPost]
        [Route("CreateGroup/{UserID}")]
        public async Task<GenericResponse> CreateGroup([FromRoute] int UserID, [FromBody] GroupCreationRequest request)
        {
            try
            {
                // Fetching the username of the user adding the expense
                string username = _usersService.GetUserNamesByUserID(UserID);
                UsersEntity usersEntity = _usersService.GetUserDetails(username);
                // Validating the group creation request
                if (request == null || string.IsNullOrEmpty(request.GroupName) || request.UserIds == null || !request.UserIds.Any())
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:RequestedNull"] };
                }
                // Creating a new group expense entity
                var group = new GroupsEntity
                {
                    GroupName = request.GroupName,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = usersEntity.Email,
                    ModifiedBy = usersEntity.Email
                };
                // Adding the group expense using the service
                var createdGroup = await _groupsService.CreateGroupAsync(group, request.UserIds);
                return new() { status = true, data = _configuration["GenericMessages:Values:CreateGroupSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:CreateGroupFail"], error = new Error() { description = ex.Message } };
            }
        }

        // Endpoint to get groups for a specific user
        [HttpGet]
        [Route("GetGroupsForUser/{userId}")]
        public async Task<IActionResult> GetGroupsForUser(int userId)
        {
            try
            {
                var groups = await _groupsService.GetGroupsForUserAsync(userId);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to get expenses for a specific group
        [HttpGet]
        [Route("GetExpensesForGroup/{groupId}")]
        public async Task<IActionResult> GetExpensesForGroup(int groupId)
        {
            try
            {
                var expenses = await _groupsService.GetExpensesForGroupAsync(groupId);
                return Ok(expenses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to add an expense to a group
        [HttpPost("AddGroupExpense/{UserID}")]
        public async Task<GenericResponse> AddGroupExpense([FromRoute] int UserID, [FromBody] GroupExpenseRequest request)
        {
            try
            {
                // Fetching the username of the user adding the expense
                string username = _usersService.GetUserNamesByUserID(UserID);
                UsersEntity usersEntity = _usersService.GetUserDetails(username);
                // Validating the group expense request
                if (request == null || request.Expense == null || request.GroupID <= 0)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:InvalidRequest"] };
                }

                var expense = request.Expense;
                expense.CreatedOn = DateTime.Now;
                expense.ModifiedOn = DateTime.Now;
                expense.CreatedBy = usersEntity.Email;
                expense.ModifiedBy = usersEntity.Email;

                var addedExpense = await _expensesService.AddExpensesAsync(expense);
                // Creating a new group expense entity
                var groupExpense = new GroupExpensesEntity
                {
                    GroupID = request.GroupID,
                    ExpenseID = addedExpense.ExpenseID,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = usersEntity.Email,
                    ModifiedBy = usersEntity.Email
                };
                // Adding the group expense using the service
                await _groupsService.AddGroupExpensesAsync(groupExpense);

                return new() { status = true, data = _configuration["GenericMessages:Values:AddGroupExpenseSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:AddGroupExpenseFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to add reconciliation expenses for a group
        [HttpPost("AddReconciliationExpense/{UserID}")]
        public async Task<GenericResponse> AddReconciliationExpense([FromRoute] int UserID, [FromBody] ReconciliationExpenseRequest request)
        {
            try
            {
                // Fetching the username of the user adding the reconciliation expense
                string username = _usersService.GetUserNamesByUserID(UserID);
                UsersEntity usersEntity = _usersService.GetUserDetails(username);
                // Validating the reconciliation expense request
                if (request == null || request.GroupID <= 0 || request.TotalAmount <= 0 || request.UserExpenses == null || !request.UserExpenses.Any())
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:InvalidReconciliation"] };
                }
                // Calculating the total amount from user expenses
                decimal totalUserAmount = request.UserExpenses.Sum(ue => ue.Amount);
                if (totalUserAmount != request.TotalAmount)
                {
                    return new() { status = false, data = _configuration["GenericMessages:Values:InSufficientReconciliation"] };
                }
                // Adding expenses for each user in the reconciliation
                foreach (var userExpense in request.UserExpenses)
                {
                    var expense = new ExpensesEntity
                    {
                        UserID = userExpense.UserID,
                        Amount = Math.Abs(userExpense.Amount),
                        Date = request.Date,
                        CategoryID = request.CategoryID,
                        ReceiptUrl = request.FileUrl,
                        Description = $"Reconciliation expense by {usersEntity.UserName}",
                        ExpenseType = userExpense.Amount < 0 ? "Credited" : "Business", //if amount is in minus then credited or else business
                        IsActive = true,
                        IsRecurring = false,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        CreatedBy = usersEntity.Email,
                        ModifiedBy = usersEntity.Email
                    };
                    // Adding the expense using the service
                    var addedExpense = await _expensesService.AddExpensesAsync(expense);
                    // Creating a new group expense entity for the reconciliation
                    var groupExpense = new GroupExpensesEntity
                    {
                        GroupID = request.GroupID,
                        ExpenseID = addedExpense.ExpenseID,
                        IsActive = true,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        CreatedBy = usersEntity.Email,
                        ModifiedBy = usersEntity.Email
                    };
                    // Adding the group expense using the service
                    await _groupsService.AddGroupExpensesAsync(groupExpense);
                }
                return new() { status = true, data = _configuration["GenericMessages:Values:ReconciliationSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:ReconciliationFail"], error = new Error() { description = ex.Message } };
            }
        }
        // Endpoint to get users in a specific group
        [HttpGet]
        [Route("GetUsersInGroup/{groupId}")]
        public async Task<IActionResult> GetUsersInGroup(int groupId)
        {
            try
            {
                var users = await _groupsService.GetUsersInGroupAsync(groupId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to get settle the owed amount to all the users in the group
        [HttpPost("SettleGroupExpenses/{UserID}")]
        public async Task<GenericResponse> SettleGroupExpenses([FromRoute] int UserID, [FromBody] SettlementRequest request)
        {
            try
            {
                // Validating the Settle Group Expenses request
                if (request == null || request.GroupID <= 0 || request.UserSettlements == null || !request.UserSettlements.Any())
                {
                    return new() { status = true, data = _configuration["GenericMessages:Values:ExpenseSettleFail"] };
                }
                await _groupsService.SettleGroupExpensesAsync(request.GroupID, request.UserSettlements);

                return new() { status = true, data = _configuration["GenericMessages:Values:ExpenseSettleSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:ExpenseSettleFail"], error = new Error() { description = ex.Message } };
            }
        }

        // Endpoint to get all settlement details in the group
        [HttpGet]
        [Route("GetUserSettlementDetails/{groupId}")]
        public async Task<IActionResult> GetUserSettlementDetails(int groupId)
        {
            try
            {
                var users = await _groupsService.GetUsersInGroupAsync(groupId);
                var expenses = await _groupsService.GetExpensesForGroupAsync(groupId);
                decimal totalSettleAmountDifference = 0;

                var userSettlements = users.Select(user => {
                    var userExpenses = expenses.Where(e => e.UserID == user.UserID); //selecting user expenses
                    var totalSpent = Math.Round(userExpenses.Where(e => e.ExpenseType != "Credited").Sum(e => e.Amount) - (userExpenses.Where(e => e.ExpenseType == "Credited").Sum(e => e.Amount)), 2,MidpointRounding.AwayFromZero); //summing the expenses and removing the credited expenses and rounding to 2 decimals
                    var totalMembers = users.Count; //count of number of memebers in the group
                    var averageShare = Math.Round(((expenses.Where(e => e.ExpenseType != "Credited").Sum(e => e.Amount) - (expenses.Where(e => e.ExpenseType == "Credited").Sum(e => e.Amount))) / totalMembers),2, MidpointRounding.AwayFromZero); //averaging all the users expenses done in group and rounding to 2 decimals
                    var totalOwed = Math.Round((averageShare - totalSpent),2,MidpointRounding.AwayFromZero); //calculate total owed
               
                    totalSettleAmountDifference += totalOwed;  //taking a variable to know the residual amount (like 0.3 etc..)
                    if (totalSettleAmountDifference > 0 && totalSettleAmountDifference < 1) //if the value exist as decimal
                        {
                            totalOwed += totalSettleAmountDifference; //adding the value to a user
                            totalSettleAmountDifference = 0; //setting to zero
                        }
                    else if (totalSettleAmountDifference > -1 && totalSettleAmountDifference < 0) //minus of decimal 
                    {
                        totalOwed -= totalSettleAmountDifference; 
                        totalSettleAmountDifference = 0;
                    }

                    return new
                    {
                        userID = user.UserID,
                        userName = user.UserName,
                        totalOwed = totalOwed,
                        expenses = new List<SettlementExpense>()
                    };
                }).ToList();

                    return Ok(userSettlements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
        // Endpoint to soft delete the group after all settlements
        [HttpDelete]
        [Route("DeleteGroup/{groupId}")]

        public async Task<GenericResponse> DeleteGroup(int groupId)
        {
            try
            {
                var settlementDetails = await _groupsService.GetUserSettlementDetailsAsync(groupId);

                bool canDelete = settlementDetails.All(user => user.NetBalance == 0);

                if (!canDelete)
                {
                    return new() { status = true, data = _configuration["GenericMessages:Values:UnsettledGroup"] };
                }

                await _groupsService.DeleteGroupAsync(groupId);

                return new() { status = true, data = _configuration["GenericMessages:Values:DeleteGroupSuccess"] };
            }
            catch (Exception ex)
            {
                return new() { status = false, data = _configuration["GenericMessages:Values:DeleteGroupFail"], error = new Error() { description = ex.Message } };
            }
        }
    }
}
