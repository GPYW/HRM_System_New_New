using System;
using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class LeaveManagement
    {
        [Key]
        [Required]
        public int? LeaveId { get; set; }

        [Required]
        public string? LeaveType { get; set; }

        public int RemainingLeaves { get; set; }

        // Navigation property to related LeaveRequestModels
        public ICollection<LeaveRequestModel>? LeaveRequests { get; set; }
    }

}


