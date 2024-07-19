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
    public class Notification
    {
        [Key]
        [Required]
        public int NotificationId { get; set; }
        [Required]
        [DisplayName("Sender Name")]
        public string SenderUsername { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string Message { get; set; } = null!;
        [Required]
        public string MessageType { get; set; } = null!;
        [Required]
        public string ReciverCode { get; set; } = null!;
        public DateTime NotificationDateTime { get; set; }
        // Foreign key to ApplicationUser
        [Required]
        [DisplayName("Sender ID")]
        public string SenderId { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("SenderId")]
        public ApplicationUser? User { get; set; }
        // Navigation property to UsersNotification
        public ICollection<UsersNotification> UsersNotifications { get; set; } = new List<UsersNotification>();
    }
}
