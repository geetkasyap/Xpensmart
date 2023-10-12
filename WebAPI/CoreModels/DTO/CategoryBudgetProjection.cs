using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class CategoryBudgetProjection
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal DesiredBudget { get; set; } 
        public decimal AdjustedBudget { get; set; }
        public decimal PreviousExpense { get; set; }
        public decimal PresentExpense { get; set; }
        public bool IsOverSpent { get; set; }
        public decimal TotalAdjustedBudget { get; set; }
        
    }
}




