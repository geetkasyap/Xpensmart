using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ExpensesService
    {
        private readonly IUnitOfWork unitOfWork;
        public ExpensesService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ExpensesEntity> AddExpensesAsync(ExpensesEntity expensesEntities)
        {
            try
            {
                return await unitOfWork.Expenses.AddExpensesAsync(expensesEntities);  
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in service while adding expense: {ex.Message}");
            }
        }

        public Task<ExpensesEntity> DeleteExpenses(int expenseID)
        {
            return unitOfWork.Expenses.DeleteExpenses(expenseID);
        }

        public List<ExpensesEntity> GetExpensesDetails()
        {
            return unitOfWork.Expenses.GetExpensesDetails();
        }

        public ExpensesEntity GetExpensesDetails(int expenseID)
        {
            return unitOfWork.Expenses.GetExpensesDetails(expenseID);
        }

        public ExpensesEntity UpdateExpenses(int ExpenseID,int UserID,ExpensesEntity updatedExpense)
        {
            return unitOfWork.Expenses.UpdateExpenses(ExpenseID,UserID, updatedExpense);
        }
        public decimal GenerateReport(int UserID, DateTime startDate, DateTime endDate, int CategoryID)
        {
            return unitOfWork.Expenses.GenerateReport(UserID,startDate,endDate,CategoryID);
        }
        public List<ExpensesEntity> GetUpcomingExpenses(int userId)
        {
            return unitOfWork.Expenses.GetUpcomingExpenses(userId);
        }
        public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            return unitOfWork.Expenses.ConvertCurrency(amount, fromCurrency, toCurrency);
        }
        public async Task ImportExpensesAsync(IEnumerable<ExpensesEntity> expenses)
        {
            try
            {
                var tasks = expenses.Select(expense =>
                    unitOfWork.Expenses.AddExpensesAsync(expense)
                ).ToList();

                await Task.WhenAll((IEnumerable<Task>)tasks);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error importing expenses: {ex.Message}", ex);
            }
        }

        //split expenses to all the users with equal percentage 
        public async Task<IActionResult> SplitExpenseAsync(ExpenseSplitRequest splitRequest, int userId , int categoryID, string FileUrl)
        {
            try
            {
                string loginEntity =unitOfWork.Users.GetEmailByUserID(userId);
                if (splitRequest.UserNames.Count != splitRequest.NumberOfMembers)
                {
                    return new BadRequestObjectResult("Number of members and usernames do not match.");
                }

                decimal individualShare = splitRequest.TotalAmount / splitRequest.NumberOfMembers;
                string Splituser = unitOfWork.Users.GetUserNamesByUserID(userId);

                var expenseEntries = new List<ExpensesEntity>();

                foreach (var userName in splitRequest.UserNames)
                {
                    var user = await unitOfWork.Users.GetUserByUserNameAsync(userName);
                    if (user == null)
                    {
                        return new BadRequestObjectResult($"User {userName} not found.");
                    }

                    var expense = new ExpensesEntity
                    {
                        UserID = user.UserID,
                        Amount = individualShare,
                        Date = DateTime.Now,
                        CategoryID = categoryID,  
                        ReceiptUrl = FileUrl,  
                        Description = $"Split expense by {Splituser}",
                        ExpenseType = "Split",  
                        IsActive = true,
                        IsRecurring = false,  
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        CreatedBy = loginEntity,
                        ModifiedBy = loginEntity
                    };

                    expenseEntries.Add(expense);
                }

                foreach (var expense in expenseEntries)
                {
                    await unitOfWork.Expenses.AddExpensesAsync(expense);
                }

                return new OkObjectResult("Expenses split and added successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while splitting expenses: {ex.Message}", ex);
            }
        }

        public List<ExpensesEntity> GetAllExpensesDetails(int userID)
        {
            return unitOfWork.Expenses.GetAllExpensesDetails(userID);
        }

        public Task AddBulkExpensesAsync(int userID, ExpensesEntity record)
        {
            return unitOfWork.Expenses.AddBulkExpensesAsync(userID,record);
        }
    }
}
