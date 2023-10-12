using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_CurrencyExchange")]
    public class CurrencyExchangeEntity
    {
        [Key]
        public int ExchangeID { get; set; }
        public string? FromCurrency { get; set; }
        public string? ToCurrency { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
