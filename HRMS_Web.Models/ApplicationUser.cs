using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HRMS_Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? join_date { get; set; }
        public string? ProfileImage { get; set; }

        // Navigation property to LeaveRequestModel
        public ICollection<LeaveRequestModel> LeaveRequests { get; set; } = new List<LeaveRequestModel>();
    }
}
