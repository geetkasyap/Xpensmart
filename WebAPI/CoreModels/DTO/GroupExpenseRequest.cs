using CoreModels.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class GroupExpenseRequest
    {
        public int GroupID { get; set; }
        public ExpensesEntity Expense { get; set; }
    }
}
