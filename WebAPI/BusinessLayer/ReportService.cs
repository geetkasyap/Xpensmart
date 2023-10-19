using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ReportService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IAggregateBudgetRepository aggregateBudgetRepository;
        public ReportService(IUnitOfWork unitOfWork, IAggregateBudgetRepository aggregateBudgetRepository)
        {
            this.unitOfWork = unitOfWork;
            this.aggregateBudgetRepository = aggregateBudgetRepository;
        }

        public decimal GenerateTotalExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            return unitOfWork.Report.GenerateTotalExpenseReport(userId, startDate, endDate, categoryId);
        }
        public decimal GenerateAverageExpenseReport(int userId, DateTime startDate, DateTime endDate, int categoryId)
        {
            return unitOfWork.Report.GenerateAverageExpenseReport(userId, startDate, endDate, categoryId);
        }

        public List<ExpensesEntity> GetRemainingExpenses(int userId)
        {
            return unitOfWork.Report.GetRemainingExpenses(userId);
        }

        public List<ExpensesEntity> GetTopThreeRecentExpenses(int userId)
        {
            return unitOfWork.Report.GetTopThreeRecentExpenses(userId);
        }

        public List<ExpensesEntity> GetUpcomingExpenses(int userId)
        {
            return unitOfWork.Report.GetUpcomingExpenses(userId);
        }

        public List<ExpensesEntity> GetExpensesByDates(int userId,DateTime startDate, DateTime endDate)
        {
            return unitOfWork.Report.GetExpensesByDates(userId,startDate,endDate);
        }

        public decimal GetBudgetByDates(int userId, DateTime startDate, DateTime endDate)
        {
            return unitOfWork.Report.GetBudgetByDates(userId, startDate, endDate);
        }

        public List<ExpensesEntity> GetCategoryExpensesByDates(int userId, DateTime startDate, DateTime endDate, int categoryID)
        {
            return unitOfWork.Report.GetCategoryExpensesByDates(userId, startDate, endDate, categoryID);
        }

        public List<CategoryExpense> GetCategorySummary(int userId, DateTime startDate, DateTime endDate)
        {
            return unitOfWork.Report.GetCategorySummary(userId, startDate, endDate);
        }
        public List<CategoryBudget> GetCategoryBudgets(int userId, DateTime start, DateTime end, int categoryid)
        {
            try
            {
                var categoryBudgets = unitOfWork.Report.GetCategoryBudgets(userId, start, end,categoryid);
                return categoryBudgets;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving category budgets: {ex.Message}");
            }
        }

        public object GetMonthlyBudgetsByDateRange(int userId, DateTime startDate, DateTime endDate)
        {
            return unitOfWork.Report.GetMonthlyBudgetsByDateRange(userId, startDate, endDate);
        }
        //calculate expense projection 
        public async Task<ExpenseProjection> CalculateExpenseProjectionAsync(int userId, decimal income)
        {
            var currentDate = DateTime.Now; //date now
            var firstDayOfPreviousMonth = new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(-1); //present year,month, date and -1 for previous date
            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1).AddDays(-1);//+1 month and -1 date
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1); //presnet month 
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1); //+1 month -1 date for month ending

            var categories = unitOfWork.Category.GetCategories(); //get cateogries
            var previousMonthExpenses = unitOfWork.Report.GetExpensesByDates(userId, firstDayOfPreviousMonth, lastDayOfPreviousMonth); //expense entities of previous month
            var presentMonthExpenses = categories.ToDictionary(
                c => c.CategoryID,
                c => unitOfWork.Report.GetCategoryExpensesByDates(userId, firstDayOfMonth, lastDayOfMonth, c.CategoryID)
                    .Sum(e => e.Amount) 
            ); //sum and ToDictionary by category id and sum

            var projection = new ExpenseProjection
            {
                UserId = userId,
                ProjectedIncome = income,
                Categories = new List<CategoryBudgetProjection>() 
            };
            decimal totalAdjustedBudget = 0; 
            foreach (var category in categories)
            {
                var previousExpense = previousMonthExpenses
                    .Where(e => e.CategoryID == category.CategoryID)
                    .Sum(e => e.Amount);

                var presentExpense = presentMonthExpenses.ContainsKey(category.CategoryID)
                    ? presentMonthExpenses[category.CategoryID]
                    : 0; //if category exists in present month set expenses or take it as zero

                var minAllocation = await GetMinimumCategoryBudget(category.CategoryID); //minumum allocation of category budget percentages
                var maxAllocation = await GetMaximumCategoryBudget(category.CategoryID); //maximum allocation of category budget percentages
                var desiredBudget = await CalculateDesiredBudget(income, category.CategoryID); //calculate desired
                var adjustedBudget = AdjustBudget(previousExpense, desiredBudget, minAllocation, maxAllocation); //adjusted
                totalAdjustedBudget = totalAdjustedBudget + adjustedBudget; 

                projection.Categories.Add(new CategoryBudgetProjection
                {
                    CategoryId = category.CategoryID,
                    CategoryName = category.Name,
                    DesiredBudget = desiredBudget,
                    AdjustedBudget = adjustedBudget,
                    PreviousExpense = previousExpense,
                    PresentExpense = presentExpense,
                    IsOverSpent = previousExpense > desiredBudget,
                    TotalAdjustedBudget = totalAdjustedBudget,
                });
            }

            return projection;
        }

        private async Task<decimal> GetMinimumCategoryBudget(int categoryId) //dictonary  with percentages and keys as catgory as key 
        {
            var categoryBudget = await aggregateBudgetRepository.GetCategoryBudgetAsync(categoryId);
            return categoryBudget?.MinimumPercentage ?? 0;
        }

        private async Task<decimal> GetMaximumCategoryBudget(int categoryId) //dictonary  with percentages and keys as catgory as key 
        {
            var categoryBudget = await aggregateBudgetRepository.GetCategoryBudgetAsync(categoryId);
            return categoryBudget?.MaximumPercentage ?? 0;
        }


        private async Task<decimal> CalculateDesiredBudget(decimal income, int categoryId) //calculate desired
        {
            var categoryBudget = await aggregateBudgetRepository.GetCategoryBudgetAsync(categoryId);
            return categoryBudget != null ? income * categoryBudget.DesiredPercentage : 0;
        }

        private decimal AdjustBudget(decimal previousExpense, decimal desiredBudget, decimal minAllocation,decimal maxAllocation) //adjust budget if previous year was not spent 
        {
            if (previousExpense > desiredBudget) 
            {
                return Math.Max(minAllocation, desiredBudget - (previousExpense - desiredBudget));
            }
            else if(previousExpense < desiredBudget)
            {
                return Math.Max(maxAllocation, desiredBudget + (previousExpense + desiredBudget));
            }
            return desiredBudget;
        }
    }
}
