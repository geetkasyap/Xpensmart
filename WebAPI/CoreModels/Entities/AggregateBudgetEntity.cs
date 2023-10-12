using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_AggregateBudget")]
    public class AggregateBudgetEntity
    {
        [Key]
        public int CategoryId { get; set; }
        public decimal MinimumPercentage { get; set; }
        public decimal MaximumPercentage { get; set; }
        public decimal DesiredPercentage { get; set; }
    }
}
