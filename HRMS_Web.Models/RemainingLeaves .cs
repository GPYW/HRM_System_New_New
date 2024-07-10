using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class RemainingLeaves
    {
        [Key]
        public int RemainingLeaveId { get; set; }

        [Required]
        public int? NoOfRemainingLeave { get; set; }

        // Foreign key to LeaveManagement
        [Required]
        public int LeaveId { get; set; }

        // Navigation property to LeaveManagement
        [ForeignKey("LeaveId")]
        public LeaveManagement LeaveManagement { get; set; }

        // Foreign key to ApplicationUser
        [Required]
        public string Id { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
    }
}