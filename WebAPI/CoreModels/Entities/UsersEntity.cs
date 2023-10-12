using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_Users")]
    public class UsersEntity
    {
        [Key]
        public int UserID { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public int GroupID { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public virtual ICollection<BudgetsEntity>? BudgetsEntities { get; set; }
        public virtual ICollection<ExpenseProjectionEntity>? ExpenseProjectionEntities { get; set; }
        public virtual ICollection<ExpensesEntity>? ExpensesEntities { get; set; }
        public virtual ICollection<NotificationsEntity>? NotificationsEntities { get; set; }
        public virtual ICollection<CategoryReport>? ReportsEntities { get; set; }
        public virtual ICollection<SharedAccountsEntity>? SentRequest { get; set; }
        public virtual ICollection<SharedAccountsEntity>? ReceivedRequest { get; set; }
        public virtual ICollection<GroupMembersEntity>? GroupMembersEntities { get; set; }

    }
}
