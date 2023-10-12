using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_Groups")]
    public class GroupsEntity
    {
        [Key]
        public int GroupID { get; set; }
        public string? GroupName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public virtual ICollection<GroupMembersEntity>? GroupMembersEntities { get; set; }
        public virtual ICollection<GroupExpensesEntity>? GroupExpensesEntities { get; set; }


    }
}
