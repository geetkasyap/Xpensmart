using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_SharedAccounts")]
    public class SharedAccountsEntity
    {
        [Key]
        public int SharedAccountID { get; set; }
        //[ForeignKey("UsersEntity")]
        public int UserID1 { get; set; }
        //[ForeignKey("UsersEntity")]
        public int UserID2 { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual UsersEntity? SentRequest { get; set; }

        public virtual UsersEntity? ReceivedRequest { get; set; }
    }
}

