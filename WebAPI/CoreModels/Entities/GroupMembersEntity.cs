using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_GroupMembers")]
    public class GroupMembersEntity
    {
        [Key]
        public int GroupMemberID { get; set; }
        [ForeignKey("GroupsEntity")]
        public int GroupID { get; set; }
        [ForeignKey("UsersEntity")]
        public int UserID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual GroupsEntity? GroupsEntity { get; set; }
        public virtual UsersEntity? UsersEntity { get; set; }

    }
}
