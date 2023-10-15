using ApplicationLayer.Interfaces;
using CoreModels.DTO;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class AggregateBudgetRepository:IAggregateBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        public AggregateBudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AggregateBudgetEntity> GetCategoryBudgetAsync(int categoryId)
        {
            try
            {
                return _context.Set<AggregateBudgetEntity>().FirstOrDefault(cb => cb.CategoryId == categoryId);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<AggregateBudgetEntity>> GetAllCategoryBudgetsAsync()
        {
            try
            {
                return _context.Set<AggregateBudgetEntity>().ToList();
            }
                catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
