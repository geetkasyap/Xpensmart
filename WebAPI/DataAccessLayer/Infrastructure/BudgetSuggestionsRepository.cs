using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class BudgetSuggestionsRepository: IBudgetSuggestionsRepository
    {
        private readonly ApplicationDbContext _context;
        public BudgetSuggestionsRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Asynchronously retrieves a list of expenses for a specific user
        public async Task<List<ExpensesEntity>> GetExpensesByUserIdAsync(int userId)
        {
            // Filtering active expenses for the given user and returning them as a list
            return await Task.FromResult(_context.Set<ExpensesEntity>().Where(e => e.UserID == userId && e.IsActive).ToList());
        }
        // Asynchronously retrieves the active budget for a specific user
        public async Task<BudgetsEntity> GetBudgetsByUserIdAsync(int userId)
        {
            return await Task.FromResult(_context.Set<BudgetsEntity>().Where(b => b.UserID == userId && b.IsActive && b.StartDate<=DateTime.Now && b.EndDate>=DateTime.Now).FirstOrDefault());
        }
        // Asynchronously retrieves a category by its ID

        public async Task<CategoriesEntity> GetCategoryByIdAsync(int categoryId)
        {
            return await Task.FromResult(_context.Set<CategoriesEntity>().FirstOrDefault(c => c.CategoryID == categoryId));
        }
        // Synchronously checks for existing budgets for a specific user
        public BudgetsEntity CheckPresentBudgets(int UserID)
        {
            try
            {
                DateTime SetDate = DateTime.Now;
                // Finding active budgets for the user that are valid for the current date
                BudgetsEntity? budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.StartDate <= SetDate && a.EndDate >= SetDate && a.UserID == UserID && a.IsActive).FirstOrDefault();
                return budgetsEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}