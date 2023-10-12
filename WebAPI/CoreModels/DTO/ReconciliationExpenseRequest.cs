using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public  class ReconciliationExpenseRequest
    {
        public int GroupID { get; set; }
        public decimal TotalAmount { get; set; } 
        public DateTime Date { get; set; }
        public int CategoryID { get; set; } 
        public string FileUrl { get; set; } 
        public string CreatedBy { get; set; } 
        public List<UserExpense> UserExpenses { get; set; }
    }
    public class UserExpense
    {
        public int UserID { get; set; } 
        public decimal Amount { get; set; } 
    }
}





