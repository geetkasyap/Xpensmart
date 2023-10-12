using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class UserSettlementDetails
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public decimal TotalOwed { get; set; }
        public List<SettlementExpense> Expenses { get; set; }
    }
}
