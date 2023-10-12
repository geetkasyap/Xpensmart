using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class UserBalance
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public decimal NetBalance { get; set; }
    }
}
