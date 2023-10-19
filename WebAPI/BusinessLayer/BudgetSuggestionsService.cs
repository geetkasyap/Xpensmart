using ApplicationLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BudgetSuggestionsService
    {
        private readonly IUnitOfWork unitOfWork;
        public BudgetSuggestionsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        //calculate budget suggestions
        public async Task<List<string>> GetBudgetSuggestionsAsync(int userId)
        {
            try
            {
                var expenses = await unitOfWork.BudgetSuggestions.GetExpensesByUserIdAsync(userId); //get expenses by userID
                var budgets = unitOfWork.BudgetSuggestions.CheckPresentBudgets(userId); //get budget by userID

                if (expenses == null || expenses.Count == 0 || budgets == null) //if request was null
                {
                    return new List<string>();
                }
                //calculating total expenses
                var totalExpenses = expenses.Where(a=>a.ExpenseType!="Credited").Sum(e => e.Amount) - expenses.Where(a => a.ExpenseType == "Credited").Sum(e => e.Amount);
                var totalBudget = budgets.Amount;
                Console.WriteLine(budgets.Amount);

                var suggestions = new List<string>();
                //budget over flow remainder
                if (totalExpenses > totalBudget)
                {
                    suggestions.Add($"You have exceeded your budget by ₹{totalExpenses - totalBudget} this month.");
                }
                else
                {
                    suggestions.Add($"You have ₹{totalBudget - totalExpenses} remaining from your budget this month.");
                }

                var categoryExpenses = expenses.GroupBy(e => e.CategoryID).ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));
                // to convert the grouped data into a dictionary.
                //the key of the dictionary will be the CategoryID
                //the value for each key in the dictionary will be the sum of the `Amount` property of all expenses in that group.
             
                foreach (var categoryExpense in categoryExpenses)
                {
                    var category = await unitOfWork.BudgetSuggestions.GetCategoryByIdAsync(categoryExpense.Key);
                    var percentageSpent = (categoryExpense.Value / totalBudget) * 100; //percentage calculation

                    if (percentageSpent > 30)
                    {
                        suggestions.Add($"You have exceeded {percentageSpent:F2}% in the {category.Name} category.");
                        suggestions.Add($"Decrease expenses in {category.Name} to be financially stable.");
                    }
                    else if (percentageSpent < 10)
                    {
                        suggestions.Add($"You can still expense more in {category.Name}.");
                    }
                }

                return suggestions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating budget suggestions: {ex.Message}");
            }
        }
    }
}
