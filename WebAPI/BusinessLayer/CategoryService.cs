using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class CategoryService
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public List<CategoriesEntity> GetCategories()
        {
            return unitOfWork.Category.GetCategories();

        }
        public async Task<CategoriesEntity> GetCategoryByNameAsync(string categoryName)
        {
            return await unitOfWork.Category.GetCategoryByNameAsync(categoryName);
        }
    }
}
