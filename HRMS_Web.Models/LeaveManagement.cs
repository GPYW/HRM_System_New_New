using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS_Web.Models
{
    public class LeaveManagement
    {
        [Key]
        [Required]
        public int LeaveId { get; set; }

        [Required]
        public string? LeaveType { get; set; }

        public int NoOfLeaves_Year { get; set; }

        // Navigation property to related LeaveRequestModels
        public ICollection<LeaveRequestModel>? LeaveRequests { get; set; }

        // Navigation property to related LeaveRequestModels
        public ICollection<RemainingLeaves>? RemainingLeaves { get; set; }
    }
}
