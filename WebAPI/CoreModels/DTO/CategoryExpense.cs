using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class CategoryExpense
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public int CategoryId { get; set; }
    }
}
