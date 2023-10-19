using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class RecurringTransactionsService
    {
        private readonly IUnitOfWork unitOfWork;
        public RecurringTransactionsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ExpensesEntity> AddTransactionsAsync(DateTime FromDate, DateTime ToDate,string ExpenseType,int CategoryID,int UserID, decimal Amount, string RecieptUrl)
        {
            return await unitOfWork.Expenses.AddTransactionsAsync(FromDate, ToDate, ExpenseType, CategoryID, UserID,Amount,RecieptUrl);
        }

        public List<ExpensesEntity> GetTransactionsDetails(string ExpenseType,int UserID)
        {
            return unitOfWork.Expenses.GetTransactionsDetails(ExpenseType, UserID);
        }

        public ExpensesEntity GetTransactionsDetails(DateTime date,string ExpenseType,int UserID)
        {
            return unitOfWork.Expenses.GetTransactionsDetails(date, ExpenseType, UserID);
        }

        public ExpensesEntity UpdateTransactions(int ExpenseID, int UserID, ExpensesEntity updatedExpense)
        {
            return unitOfWork.Expenses.UpdateTransactions(ExpenseID, UserID, updatedExpense);
        }
        public ExpensesEntity DeleteTransactions(int ExpenseID)
        {
            return unitOfWork.Expenses.DeleteTransactions(ExpenseID);
        }
    }
}
