using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class BudgetService
    {
        private readonly IUnitOfWork unitOfWork;
        public BudgetService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public BudgetsEntity AddBudget(BudgetsEntity budgetsEntities)
        {
            return unitOfWork.Budget.AddBudget(budgetsEntities);
        }

        public BudgetsEntity DeleteBudget(int budgetID)
        {
            return unitOfWork.Budget.DeleteBudget(budgetID);
        }

        public List<BudgetsEntity> GetBudgetDetails()
        {
            return unitOfWork.Budget.GetBudgetDetails();
        }

        public BudgetsEntity GetBudgetDetails(int budgetID)
        {
            return unitOfWork.Budget.GetBudgetDetails(budgetID);
        }

        public BudgetsEntity UpdateBudget(int budgetID, decimal amount)
        {
            return unitOfWork.Budget.UpdateBudget(budgetID,amount);
        }
        public List<BudgetsEntity> GetBudgetsByUser(int UserID)
        {
            return unitOfWork.Budget.GetBudgetsByUser(UserID);
        }
        public BudgetsEntity CheckPreviousBudgets(DateTime StartDate, DateTime EndDate, int UserID)
        {
            return unitOfWork.Budget.CheckPreviousBudgets(StartDate,EndDate,UserID);
        }


    }
}
