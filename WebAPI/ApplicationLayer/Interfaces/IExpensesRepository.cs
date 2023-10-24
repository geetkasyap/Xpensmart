using CoreModels.Entities;
using CoreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IExpensesRepository
    {
        Task<ExpensesEntity> AddExpensesAsync(ExpensesEntity expensesEntities);
        Task<ExpensesEntity> DeleteExpenses(int expenseID);
        List<ExpensesEntity> GetExpensesDetails();
        ExpensesEntity GetExpensesDetails(int expenseID);
        ExpensesEntity UpdateExpenses(int ExpenseID,int UserID ,ExpensesEntity updatedExpense);
        public decimal GenerateReport(int UserID, DateTime startDate, DateTime endDate, int CategoryID);
        List<ExpensesEntity> GetUpcomingExpenses(int userId);
        public decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency);


        Task<ExpensesEntity> AddTransactionsAsync(DateTime FromDate, DateTime ToDate, string ExpenseType, int CategoryID, int UserID, decimal Amount, string RecieptUrl);
        List<ExpensesEntity> GetTransactionsDetails(string expenseType, int userID);
        ExpensesEntity GetTransactionsDetails(DateTime date, string ExpenseType, int UserID);
        ExpensesEntity UpdateTransactions(int ExpenseID, int UserID, ExpensesEntity updatedExpense);
        ExpensesEntity DeleteTransactions(int ExpenseID);
        List<ExpensesEntity> GetAllExpensesDetails(int userID);
        Task AddBulkExpensesAsync(int UserID, ExpensesEntity expensesEntity);
    }
}
