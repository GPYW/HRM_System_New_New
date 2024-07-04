using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Web.Models
{
    public class LeaveRequestModel
    {
        [Key]
        [Required]
        public int? RequestId { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        public int NumberOfDays => (ToDate - FromDate).Days + 1;

        [Required]
        [StringLength(500)]
        public string? LeaveReason { get; set; }

        [Required]
        public string? Status { get; set; } // "Pending", "Approved", "Declined"

        // Foreign key to LeaveManagement
        [Required]
        public int LeaveId { get; set; }

        // Navigation property to LeaveManagement
        public LeaveManagement? LeaveManagement { get; set; }

        // Foreign key to ApplicationUser
        [Required]
        public string Id { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("Id")]
        public ApplicationUser User { get; set; }
    }
}
