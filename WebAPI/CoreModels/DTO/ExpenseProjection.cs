using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class ExpenseProjection
    {
        public int UserId { get; set; }
        public decimal ProjectedIncome { get; set; }
        public List<CategoryBudgetProjection> Categories { get; set; }
    }
}
