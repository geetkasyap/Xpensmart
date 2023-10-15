using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Infrastructure
{
    public class CategoryKeywordsRepository: ICategoryKeywordsRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryKeywordsRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        // Method to retrieve category keywords
        public IEnumerable<CategoryKeywordsEntity> GetCategoryKeywords()
        {
            return _context.Set<CategoryKeywordsEntity>()  // Accessing the CategoryKeywordsEntity 
                .Include(kw => kw.CategoriesEntity)   // Including related CategoriesEntity for eager loading
                .Select(kw => new CategoryKeywordsEntity // Projecting the results into a new CategoryKeywordsEntity
                {
                    CategoryKeywordID = kw.CategoryKeywordID,  
                    CategoryID = kw.CategoryID, // Mapping the CategoryID
                    Keyword = kw.Keyword,  // Mapping the Keyword
                    CategoriesEntity = kw.CategoriesEntity // Mapping the related 
                })
                .ToList();
        }
    }
}
