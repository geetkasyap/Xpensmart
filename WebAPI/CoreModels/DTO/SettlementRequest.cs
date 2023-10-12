using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class SettlementRequest
    {
        public int GroupID { get; set; }
        public List<UserSettlement> UserSettlements { get; set; }
    }
}
