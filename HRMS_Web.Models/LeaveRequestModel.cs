using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Web.Models
{
    public class LeaveRequestModel
    {
        [Key]
        public int RequestId { get; set; }

        public string? LeaveDuration { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int NumberOfLeaveDays { get; set; }

        [Required]
        [StringLength(500)]
        public string LeaveReason { get; set; }

        [Required]
        public string Status { get; set; } // "Pending", "Approved", "Declined"

        // Foreign key to LeaveManagement
        [Required]
        public int? LeaveId { get; set; }

        // Navigation property to LeaveManagement
        [ForeignKey("LeaveId")]
        public LeaveManagement? LeaveManagement { get; set; }

        // Foreign key to ApplicationUser
        [Required]
        public string Id { get; set; }

        // Navigation property to ApplicationUser
        [ForeignKey("Id")]
        public ApplicationUser? User { get; set; }
        [Required]
        public string? LeaveType { get; set; }
    }
}