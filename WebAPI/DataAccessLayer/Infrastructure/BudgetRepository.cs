using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataAccessLayer.Infrastructure
{
    public class BudgetRepository: IBudgetRepository
    {
        private readonly ApplicationDbContext _context;
        public BudgetRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Method to add a new budget
        public BudgetsEntity AddBudget(BudgetsEntity budgetsEntities)
        {
            try
            {
                // Retrieve the user entity based on UserID
                UsersEntity loginEntity = _context.Set<UsersEntity>().Where(a => a.UserID == budgetsEntities.UserID && a.IsActive).FirstOrDefault();
                if (loginEntity == null)
                {
                    throw new Exception("User  not found.");
                }
                BudgetsEntity newBudgets = new BudgetsEntity
                {
                    UserID = budgetsEntities.UserID,
                    Amount = budgetsEntities.Amount,
                    StartDate = budgetsEntities.StartDate,
                    EndDate = budgetsEntities.EndDate,
                    Frequency = budgetsEntities.Frequency,
                    IsActive = true,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    CreatedBy = loginEntity.Email,
                    ModifiedBy = loginEntity.Email,
                };
                _context.Set<BudgetsEntity>().Add(newBudgets);
                _context.SaveChanges();
                return newBudgets;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to delete a budget by setting its IsActive property to false
        public BudgetsEntity DeleteBudget(int budgetID)
        {
            try
            {
                BudgetsEntity? budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.BudgetID == budgetID && a.IsActive).FirstOrDefault();
                budgetsEntities.IsActive = false;
                _context.SaveChanges();
                return budgetsEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to retrieve all active budgets
        public List<BudgetsEntity> GetBudgetDetails()
        {
            try
            {
                List<BudgetsEntity> budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.IsActive).ToList();
                return budgetsEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to retrieve a specific budget by its ID
        public BudgetsEntity GetBudgetDetails(int budgetID)
        {
            try
            {
                BudgetsEntity budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.BudgetID == budgetID && a.IsActive).FirstOrDefault();
                return budgetsEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to retrieve budgets for a specific user
        public List<BudgetsEntity> GetBudgetsByUser(int UserID)
        {
            try
            {
                List<BudgetsEntity> budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.IsActive && a.UserID== UserID).ToList();
                return budgetsEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Method to update the amount of a specific budget
        public BudgetsEntity UpdateBudget(int budgetID, decimal amount)
        {
            try
            {
                BudgetsEntity? budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.BudgetID == budgetID && a.IsActive).FirstOrDefault();
                budgetsEntities.Amount = amount;
                budgetsEntities.ModifiedOn = DateTime.Now;
                _context.SaveChanges();
                return budgetsEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // Method to check for previous budgets within a date range for a specific user
        public BudgetsEntity CheckPreviousBudgets(DateTime StartDate , DateTime EndDate , int UserID)
        {
            try
            {
                BudgetsEntity? budgetsEntities = _context.Set<BudgetsEntity>().Where(a => a.StartDate == StartDate && a.EndDate==EndDate && a.UserID==UserID && a.IsActive).FirstOrDefault();
                return budgetsEntities;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
