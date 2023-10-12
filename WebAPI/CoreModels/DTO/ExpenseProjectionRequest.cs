using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class ExpenseProjectionRequest
    {
        public int UserId { get; set; }
        public decimal Income { get; set; }
    }
}
