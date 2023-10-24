using CoreModels.DTO;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IReportRepository
    {
        decimal GenerateTotalExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId);
        decimal GenerateAverageExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId);
        List<ExpensesEntity> GetRemainingExpenses(int userId);
        List<ExpensesEntity> GetTopThreeRecentExpenses(int userId);
        List<ExpensesEntity> GetUpcomingExpenses(int userId);
        List<ExpensesEntity> GetExpensesByDates(int userId, DateTime startDate, DateTime endDate);
        decimal GetBudgetByDates(int userId, DateTime startDate, DateTime endDate);
        List<ExpensesEntity> GetCategoryExpensesByDates(int userId, DateTime startDate, DateTime endDate, int categoryID);
        List<CategoryExpense> GetCategorySummary(int userId, DateTime startDate, DateTime endDate);
        public List<CategoryBudget> GetCategoryBudgets(int userId, DateTime start, DateTime end, int categoryid);
        List<MonthlyBudgets> GetMonthlyBudgetsByDateRange(int userId, DateTime startDate, DateTime endDate);
    }
}
