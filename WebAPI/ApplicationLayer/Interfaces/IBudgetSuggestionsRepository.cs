using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IBudgetSuggestionsRepository
    {
        public Task<List<ExpensesEntity>> GetExpensesByUserIdAsync(int userId);
        public Task<BudgetsEntity> GetBudgetsByUserIdAsync(int userId);
        public Task<CategoriesEntity> GetCategoryByIdAsync(int categoryId);
        public BudgetsEntity CheckPresentBudgets(int UserID);

    }
}
