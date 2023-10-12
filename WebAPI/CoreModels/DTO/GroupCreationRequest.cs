using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DTO
{
    public class GroupCreationRequest
    {
        public string GroupName { get; set; }
        public List<int> UserIds { get; set; }
    }
}
