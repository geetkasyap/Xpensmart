using ApplicationLayer.Interfaces;
using CoreModels;
using CoreModels.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class ExpensesRepository : IExpensesRepository
    {
        private readonly ApplicationDbContext _context;
        public ExpensesRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Method to add a new expense asynchronously
        public async Task<ExpensesEntity> AddExpensesAsync(ExpensesEntity expensesEntities)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == expensesEntities.UserID && a.IsActive).FirstOrDefault();
                ExpensesEntity newExpenses = new ExpensesEntity
                {
                    UserID = expensesEntities.UserID,
                    Amount = expensesEntities.Amount,
                    Date = expensesEntities.Date,
                    CategoryID = expensesEntities.CategoryID,
                    ReceiptUrl = expensesEntities.ReceiptUrl,
                    Description = expensesEntities.Description,
                    ExpenseType = expensesEntities.ExpenseType,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = loginEntity.Email,
                    ModifiedBy = loginEntity.Email,
                };
                _context.Set<ExpensesEntity>().Add(newExpenses);
                await _context.SaveChangesAsync();
                return newExpenses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Method to delete an expense by setting IsActive to false
        public async Task<ExpensesEntity> DeleteExpenses(int expenseID)
        {
            try
            {
                ExpensesEntity? expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.ExpenseID == expenseID && a.IsActive).FirstOrDefault();
                expensesEntities.IsActive = false;
                await _context.SaveChangesAsync();
                return expensesEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get all active expenses
        public List<ExpensesEntity> GetExpensesDetails()
        {
            try
            {
                List<ExpensesEntity> expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.IsActive).ToList();
                return expensesEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get details of a specific expense by ExpenseID
        public ExpensesEntity GetExpensesDetails(int expenseID)
        {
            try
            {
                ExpensesEntity expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.ExpenseID == expenseID && a.IsActive).FirstOrDefault();
                return expensesEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to update an existing expense
        public ExpensesEntity UpdateExpenses(int ExpenseID,int UserID,ExpensesEntity updatedExpense)
        {
            // Retrieve the user entity based on UserID
            UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == UserID && a.IsActive).FirstOrDefault();
            // Find the expense entity by ExpenseID
            ExpensesEntity? expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.ExpenseID == ExpenseID && a.IsActive).FirstOrDefault();
            expensesEntities.Amount = updatedExpense.Amount;
            expensesEntities.Date = updatedExpense.Date;
            expensesEntities.CategoryID = updatedExpense.CategoryID;
            expensesEntities.ReceiptUrl = updatedExpense.ReceiptUrl;
            expensesEntities.Description = updatedExpense.Description;
            expensesEntities.ExpenseType = updatedExpense.ExpenseType;
            expensesEntities.ModifiedBy = loginEntity.Email;
            expensesEntities.ModifiedOn = DateTime.Now;
            _context.SaveChanges();
            return expensesEntities;

        }
        // Method to generate a report for a specific category within a date range
        public decimal GenerateReport(int UserID, DateTime startDate, DateTime endDate, int CategoryID)
        {
            // Calculate the total amount for the specified category and date range
            decimal categoryReport = _context.Set<ExpensesEntity>()
                .Where(e => e.UserID == UserID && e.Date >= startDate && e.Date <= endDate && e.CategoryID == CategoryID && e.IsActive)
                .Select(e => e.ExpenseType == "Credited" ? -e.Amount : e.Amount)
                .Sum();
            return categoryReport;
        }
        // Method to get upcoming expenses for a specific user
        public List<ExpensesEntity> GetUpcomingExpenses(int userId)
        {
            try
            {
                // Retrieve all upcoming expenses for the user
                List<ExpensesEntity> expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.IsActive && a.ExpenseType=="UpComing" && a.UserID==userId).ToList();
                return expensesEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to convert currency based on exchange rates
        public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            try
            {
                // Determine the currency to exchange
                CurrencyExchangeEntity exchangeRate;
                if (fromCurrency != "INR")
                {
                    exchangeRate = _context.Set<CurrencyExchangeEntity>().Where(e => (e.FromCurrency == fromCurrency && e.ToCurrency == toCurrency) && e.IsActive).FirstOrDefault();
                }
                else
                {
                    exchangeRate = _context.Set<CurrencyExchangeEntity>().Where(e => (e.FromCurrency == toCurrency && e.ToCurrency == fromCurrency) && e.IsActive).FirstOrDefault();

                }
                if (exchangeRate!=null)
                {
                    return amount * exchangeRate.Rate;
                }
                else
                {
                    return amount * 1; // Return the original amount if no exchange rate is found
                }

            }
            catch
            {
                throw new Exception("Currency conversion rate not found.");
            }


        }
        // Method to add recurring transactions asynchronously
        public async Task<ExpensesEntity> AddTransactionsAsync(DateTime FromDate, DateTime ToDate, string ExpenseType, int CategoryID, int UserID, decimal Amount,string RecieptUrl)
        {
            try
            {
                // Retrieve the user entity based on UserID
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID ==UserID && a.IsActive).FirstOrDefault();
                List<ExpensesEntity> recurringExpenses = new List<ExpensesEntity>();
                DateTime currentDate = FromDate;
                // Loop through the date range to create recurring expenses
                while (currentDate <= ToDate)
                {
                    if ((ExpenseType.Contains("Monthly") && currentDate.Day == 1) ||
                        (ExpenseType.Contains("Yearly") && currentDate.Month == 1 && currentDate.Day == 1))
                    {
                        recurringExpenses.Add(new ExpensesEntity
                        {
                            UserID = UserID,
                            Amount = Amount,
                            Date = currentDate,
                            CategoryID = CategoryID,
                            ReceiptUrl= RecieptUrl,
                            Description="Auto Recurring Expenses",
                            ExpenseType = ExpenseType,
                            IsActive = true,
                            IsRecurring = true,
                            CreatedOn = DateTime.Now,
                            ModifiedOn = DateTime.Now,
                            CreatedBy = loginEntity.Email,
                            ModifiedBy = loginEntity.Email
                        });
                        currentDate = ExpenseType.Contains("Monthly")
                            ? currentDate.AddMonths(1)
                            : currentDate.AddYears(1);
                    }
                    else
                    {
                        currentDate = currentDate.AddDays(-1);
                    }
                }

                await _context.Set<ExpensesEntity>().AddRangeAsync(recurringExpenses);
                await _context.SaveChangesAsync();

                return recurringExpenses.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get details of recurring transactions for a user
        public List<ExpensesEntity> GetTransactionsDetails(string expenseType, int userID)
        {
            try
            {
                return _context.Set<ExpensesEntity>()
                    .Where(e => e.UserID == userID && e.IsActive && e.IsRecurring)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get details of a specific transaction by date and type
        public ExpensesEntity GetTransactionsDetails(DateTime date, string ExpenseType, int UserID)
        {
            try
            {
                // Find the transaction based on date, expense type, and user ID
                return _context.Set<ExpensesEntity>()
                    .Where(e => e.Date == date && e.ExpenseType == ExpenseType && e.UserID == UserID && e.IsActive)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to update recurring transactions
        public ExpensesEntity UpdateTransactions(int ExpenseID, int UserID, ExpensesEntity updatedExpense)
        {
            try
            {
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == UserID && a.IsActive).FirstOrDefault();
                ExpensesEntity transactionToUpdate = _context.Set<ExpensesEntity>()
                    .FirstOrDefault(e => e.ExpenseID == ExpenseID && e.IsActive && e.IsRecurring);

                if (transactionToUpdate == null)
                {
                    throw new Exception("transactionToUpdate was null");

                }

                var selectedTransactions = _context.Set<ExpensesEntity>()
                    .Where(e => e.IsActive && e.IsRecurring &&
                                e.CreatedOn.Date == transactionToUpdate.CreatedOn.Date &&
                                e.CreatedOn.Second == transactionToUpdate.CreatedOn.Second)
                    .ToList();
                // Update each selected transaction
                foreach (var selection in selectedTransactions)
                {
                    selection.Amount = updatedExpense.Amount;
                    selection.Date = updatedExpense.Date;
                    selection.CategoryID = updatedExpense.CategoryID;
                    selection.ReceiptUrl = updatedExpense.ReceiptUrl;
                    selection.Description = updatedExpense.Description;
                    selection.ExpenseType = updatedExpense.ExpenseType;
                    selection.ModifiedBy = loginEntity.Email;
                    selection.ModifiedOn = DateTime.Now;
                }

                _context.SaveChanges();

                return selectedTransactions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to delete recurring transactions
        public ExpensesEntity DeleteTransactions(int ExpenseID)
        {
            try
            {
                ExpensesEntity transactionToDelete = _context.Set<ExpensesEntity>()
                    .FirstOrDefault(e => e.ExpenseID == ExpenseID && e.IsActive && e.IsRecurring);

                if (transactionToDelete == null)
                {
                    throw new Exception("transactionToDelete was null");

                }

                var selectedTransactions = _context.Set<ExpensesEntity>()
                    .Where(e => e.IsActive && e.IsRecurring &&
                                e.CreatedOn.Date == transactionToDelete.CreatedOn.Date &&
                                e.CreatedOn.Second == transactionToDelete.CreatedOn.Second)
                    .ToList();

                foreach (var selection in selectedTransactions)
                {
                    selection.IsActive = false;
                }

                _context.SaveChanges();

                return selectedTransactions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get all expenses for a specific user
        public List<ExpensesEntity> GetAllExpensesDetails(int userID)
        {
            try
            {
                List<ExpensesEntity> expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.IsActive && a.UserID==userID).ToList();
                return expensesEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to add bulk expenses asynchronously
        public async Task AddBulkExpensesAsync(int UserID, ExpensesEntity expensesEntity)
        {
            try
            {
                var user = _context.Set<UsersEntity>().Where(u => u.UserID == UserID && u.IsActive).FirstOrDefault();
                if (user == null)
                {
                    throw new Exception("User not found or inactive.");
                }

                var newExpense = new ExpensesEntity
                {
                    UserID = UserID,
                    Amount = expensesEntity.Amount,
                    Date = expensesEntity.Date,
                    CategoryID = expensesEntity.CategoryID,
                    ReceiptUrl = expensesEntity.ReceiptUrl,
                    Description = expensesEntity.Description,
                    ExpenseType = expensesEntity.ExpenseType,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = user.Email,
                    ModifiedBy = user.Email
                };

                _context.Set<ExpensesEntity>().Add(newExpense);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
