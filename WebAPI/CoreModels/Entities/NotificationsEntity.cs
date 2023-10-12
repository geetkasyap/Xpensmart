using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.Entities
{
    [Table("Expensmart_Tbl_Notifications")]
    public class NotificationsEntity
    {
        [Key]
        public int NotificationID { get; set; }
        [ForeignKey("UsersEntity")]
        public int UserID { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; }
        public bool IsActive { get; set; }
        public bool ReadMessage { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public virtual UsersEntity? UsersEntity { get; set; }
    }
}
