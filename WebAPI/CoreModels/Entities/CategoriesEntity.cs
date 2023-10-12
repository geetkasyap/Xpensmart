using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_Categories")]
    public class CategoriesEntity
    {
        [Key]
        public int CategoryID { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual ICollection<ExpensesEntity>? ExpensesEntities { get; set; }
        public virtual ICollection<CategoryKeywordsEntity>? CategoryKeywordsEntities { get; set; }
    }
}
