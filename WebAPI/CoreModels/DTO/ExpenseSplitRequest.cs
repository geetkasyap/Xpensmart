using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoreModels.DTO
{
    public class ExpenseSplitRequest
    {
        public decimal TotalAmount { get; set; }
        public int NumberOfMembers { get; set; }
        public List<string> UserNames { get; set; }
    }
}
