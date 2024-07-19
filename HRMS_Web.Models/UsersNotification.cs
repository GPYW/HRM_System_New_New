using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class UsersNotification
    {
        [Key]
        [Required]
        public int RecordId { get; set; }
        [Required]
        public string RecieverId { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("RecieverId")]
        public ApplicationUser? User { get; set; }
        [Required]
        public int NotificationId { get; set; }

        // Navigation property to Notification
        [ForeignKey("NotificationId")]
        public Notification? Notification { get; set; }
        public Boolean IsRead { get; set; }

    }
}
