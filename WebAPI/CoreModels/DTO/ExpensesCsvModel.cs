using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class ExpensesCsvModel
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? Description { get; set; }
        public string? ExpenseType { get; set; }
    }
}
