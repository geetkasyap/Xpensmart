using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;
        public ReportRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Method to generate total expense report for a user within a date range and category
        public decimal GenerateTotalExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            try
            {
                decimal totalAmount = 0;// Initialize total amount
                if (categoryId != 0)
                {
                    // Fetch expenses for the specified user, date range, and category
                    var expenses = _context.Set<ExpensesEntity>()
    .Where(e => e.UserID == userId
                && e.Date >= startDate
                && e.Date <= endDate
                && e.CategoryID == categoryId
                && e.IsActive
    ).ToList();
                    // Get the name of the category to ensure it exists
                    var categoryName = _context.Set<CategoriesEntity>()
                        .Where(c => c.CategoryID == categoryId && c.IsActive)
                        .Select(c => c.Name)
                        .FirstOrDefault();

                    if (categoryName == null)
                    {
                        throw new Exception("Category not found or inactive.");
                    }
                    // Calculate total amount based on expense type
                    foreach (var expense in expenses)
                    {
                        if (expense.ExpenseType.ToLower() == "credited")
                        {
                            totalAmount -= expense.Amount;
                        }
                        else
                        {
                            totalAmount += expense.Amount;
                        }
                    }
                    return totalAmount;
                }
                else
                {
                    var expenses = _context.Set<ExpensesEntity>()
    .Where(e => e.UserID == userId
                && e.Date >= startDate
                && e.Date <= endDate
                && e.IsActive
    ).ToList();

                    // Calculate total amount for all expenses
                    foreach (var expense in expenses)
                    {
                        if (expense.ExpenseType?.ToLower() == "credited")
                        {
                            totalAmount -= expense.Amount;// Subtract credited amounts
                        }
                        else
                        {
                            totalAmount += expense.Amount; // Add debited amounts
                        }
                    }

                    return totalAmount;
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }
        // Method to generate average expense report for a user within a date range and category

        public decimal GenerateAverageExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            try
            {
                // Fetch expenses for the specified user, date range, and category

                var expenses = _context.Set<ExpensesEntity>()
                    .Where(e => e.UserID == userId
                                && e.Date >= startDate
                                && e.Date <= endDate
                                && e.CategoryID == categoryId
                                && e.IsActive
                    ).ToList();
                // Get the name of the category to ensure it exists

                var categoryName = _context.Set<CategoriesEntity>()
                    .Where(c => c.CategoryID == categoryId && c.IsActive)
                    .Select(c => c.Name)
                    .FirstOrDefault();

                if (categoryName == null)
                {
                    throw new Exception("Category not found or inactive.");
                }

                decimal totalAmt = 0;
                decimal AverageAmount = 0;
                int CountOfExpenses = 0;
                // Calculate total amount and count of expenses
                foreach (var expense in expenses)
                {
                    if (expense.ExpenseType.ToLower() == "credited")
                    {
                        totalAmt -= expense.Amount;
                    }
                    else
                    {
                        totalAmt += expense.Amount;
                    }
                    CountOfExpenses = CountOfExpenses + 1;
                }
                AverageAmount = totalAmt / CountOfExpenses;

                return AverageAmount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating report: {ex.Message}");
            }
        }
        // Method to retrieve remaining expenses for a user
        public List<ExpensesEntity> GetRemainingExpenses(int userId)
        {
            try
            {
                var RemainingExpenseList = _context.Set<ExpensesEntity>()
                    .Where(e => e.UserID == userId && e.IsActive==true && e.ExpenseType.ToLower() == "Need To Be Paid").Take(5)
                    .ToList();
                return RemainingExpenseList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching remaining expenses: {ex.Message}");
            }
        }
        // Method to get the top three recent expenses for a user
        public List<ExpensesEntity> GetTopThreeRecentExpenses(int userId)
        {
            try
            {
                return _context.Set<ExpensesEntity>()
                    .Where(e => e.UserID == userId && e.IsActive)
                    .OrderByDescending(e => e.ModifiedOn)
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching recent expenses: {ex.Message}");
            }
        }
        // Method to get upcoming expenses for a user
        public List<ExpensesEntity> GetUpcomingExpenses(int userId)
        {
            try
            {
                var currentDate = DateTime.Now;
                return _context.Set<ExpensesEntity>()
                    .Where(e => e.UserID == userId && e.IsActive == true &&(e.ExpenseType.ToLower() == "UpComing" ||
                                 (e.IsRecurring == true && (e.Date.Month > currentDate.Month || e.Date.Year > currentDate.Year))
                                 )).Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching upcoming expenses: {ex.Message}");
            }
        }
        // Method to get expenses by date range for a user
        public List<ExpensesEntity> GetExpensesByDates(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<ExpensesEntity> expensesEntities = _context.Set<ExpensesEntity>()
                    .Include(a => a.CategoriesEntity)
                    .Where(a => a.UserID == userId && a.Date >= startDate
                                && a.Date <= endDate && a.IsActive).ToList();
                return expensesEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get budget by date range for a user
        public decimal GetBudgetByDates(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                BudgetsEntity budgetEntities = _context.Set<BudgetsEntity>().Where(a => a.UserID == userId && a.StartDate >= startDate
                                && a.EndDate <= endDate && a.IsActive && a.Frequency == "Monthly").FirstOrDefault();
                if (budgetEntities == null)
                {
                    return 0;
                }
                else
                {
                    return budgetEntities.Amount;
                }
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get category expenses by date range for a user
        public List<ExpensesEntity> GetCategoryExpensesByDates(int userId, DateTime startDate, DateTime endDate, int categoryID)
        {
            try
            {
                List<ExpensesEntity> expensesEntities = _context.Set<ExpensesEntity>().Where(a => a.UserID == userId && a.Date >= startDate
                                && a.Date <= endDate && a.IsActive && a.CategoryID == categoryID).ToList();
                return expensesEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get category summary of expenses for a user
        public List<CategoryExpense> GetCategorySummary(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var categoryExpenses = _context.Set<ExpensesEntity>()
                    .Include(e => e.CategoriesEntity)
                    .Where(e => e.UserID == userId
                        && e.Date >= startDate
                        && e.Date <= endDate
                        && e.IsActive)
                    .GroupBy(e => new { e.CategoryID, e.CategoriesEntity.Name })
                    .Select(g => new CategoryExpense
                    {
                        CategoryId = g.Key.CategoryID,
                        CategoryName = g.Key.Name,
                        Amount = g.Sum(e => e.ExpenseType.ToLower() == "credited" ? -e.Amount : e.Amount)
                    })
                    .ToList();// Calculate total amount per category
                return categoryExpenses;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to get category budgets for a user within a date range
        public List<CategoryBudget> GetCategoryBudgets(int userId, DateTime start, DateTime end, int categoryid)
        {
            try
            {
                decimal categoryAmount = GenerateTotalExpenseReport(userId, start, end, categoryid);
                var budgets = _context.Set<BudgetsEntity>()
                    .Where(b => b.UserID == userId && b.StartDate >= start && b.EndDate <= end && b.IsActive)
                    .ToList();

                var categoryBudgets = budgets.Select(b => new CategoryBudget
                {
                    Amount = categoryAmount,
                    CategoryId = categoryid,
                    BudgetAmount = b.Amount,
                    Percentage = (b.Amount > 0) ? (b.Amount / b.Amount) * 100 : 0
                }).ToList();

                return categoryBudgets;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving category budgets: {ex.Message}");
            }
        }
        // Method to get monthly budgets for a user within a date range
        public List<MonthlyBudgets> GetMonthlyBudgetsByDateRange(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var monthlyBudgets = _context.Set<BudgetsEntity>()
                    .Where(b => b.UserID == userId &&
                           b.IsActive &&
                           b.Frequency == "Monthly" &&
                           b.StartDate <= endDate &&
                           b.EndDate >= startDate)
                    .Select(b => new MonthlyBudgets
                    {
                        Month = b.StartDate.ToString("yyyy-MM"),// Format month as yyyy-MM
                        Amount = b.Amount
                    })
                    .ToList();

                return monthlyBudgets;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching monthly budgets: {ex.Message}");
            }
        }

    }
}
