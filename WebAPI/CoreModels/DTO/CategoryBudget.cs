using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class CategoryBudget
    {
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal Percentage { get; set; }
    }
}
