using ApplicationLayer.Interfaces;
using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class CategoryKeywordsService
    {
        private readonly IUnitOfWork unitOfWork;
        public CategoryKeywordsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<CategoryKeywordsEntity> GetCategoryKeywords()
        {
            return unitOfWork.CategoryKeywords.GetCategoryKeywords();
        }
    }
}
