using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class SettlementExpense
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string FileUrl { get; set; }
    }
}
