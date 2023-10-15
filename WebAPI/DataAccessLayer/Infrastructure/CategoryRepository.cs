using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class CategoryRepository:ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Method to retrieve a list of active categories
        public List<CategoriesEntity> GetCategories()
        {
            try
            {
                // Fetching active categories from the database
                List<CategoriesEntity> categoriesEntities = _context.Set<CategoriesEntity>().Where(a => a.IsActive).ToList();
                return categoriesEntities;
            }
            catch (Exception ex)

            {
                throw new Exception(ex.Message);
            }
        }
        // Asynchronous method to retrieve a category by its name
        public async Task<CategoriesEntity> GetCategoryByNameAsync(string categoryName)
        {
            // Fetching the category that matches the provided name and is active
            CategoriesEntity categoriesEntities = _context.Set<CategoriesEntity>().Where(c => c.Name == categoryName && c.IsActive).FirstOrDefault();
            return categoriesEntities;
        }
    }
}
