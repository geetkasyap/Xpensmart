using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_Expenses")]
    public class ExpensesEntity
    {
        [Key]
        public int ExpenseID { get; set; }
        [ForeignKey("UsersEntity")]
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        [ForeignKey("CategoriesEntity")]
        public int CategoryID { get; set; }
        public string? ReceiptUrl { get; set; }
        public string? Description { get; set; }
        public string? ExpenseType { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecurring { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public virtual UsersEntity? UsersEntity { get; set; }
        public virtual CategoriesEntity? CategoriesEntity { get; set; }
        public virtual ICollection<GroupExpensesEntity>? GroupExpensesEntities { get; set; }
    }
}
