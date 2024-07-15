using Microsoft.AspNetCore.Identity;

namespace HRMS_Web.Models
{
    public class ConfigurationViewModel
    {
        public List<LeaveType> LeaveTypes { get; set; }
        public LeaveType NewLeaveType { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public IdentityRole NewRole { get; set; }
        public List<Department> Departments { get; set; }
        public Department NewDepartment { get; set; }
    }
}
