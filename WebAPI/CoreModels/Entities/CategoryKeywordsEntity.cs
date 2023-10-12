using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_CategoryKeywords")]
    public class CategoryKeywordsEntity
    {
        [Key]
        public int CategoryKeywordID { get; set; }
        [ForeignKey("CategoriesEntity")]
        public int CategoryID { get; set; }
        public string Keyword { get; set; }
        public virtual CategoriesEntity? CategoriesEntity { get; set; }
    }
}
