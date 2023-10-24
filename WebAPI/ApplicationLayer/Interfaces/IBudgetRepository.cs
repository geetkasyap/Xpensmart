using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces
{
    public interface IBudgetRepository
    {
        BudgetsEntity AddBudget(BudgetsEntity budgetsEntities);
        BudgetsEntity DeleteBudget(int budgetID);
        List<BudgetsEntity> GetBudgetDetails();
        BudgetsEntity GetBudgetDetails(int budgetID);
        List<BudgetsEntity> GetBudgetsByUser(int UserID);
        BudgetsEntity UpdateBudget(int budgetID, decimal amount);
        BudgetsEntity CheckPreviousBudgets(DateTime StartDate, DateTime EndDate, int UserID);

    }
}
