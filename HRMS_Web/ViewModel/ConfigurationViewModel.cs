using Microsoft.AspNetCore.Identity;

namespace HRMS_Web.Models
{
    public class ConfigurationViewModel
    {
        public List<LeaveManagement> LeaveTypes { get; set; }
        public LeaveManagement NewLeaveType { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public IdentityRole NewRole { get; set; }
        public List<Department> Departments { get; set; }
        public Department NewDepartment { get; set; }
        public List<Admin> Admins { get; set; }
        public Admin NewAdmin { get; set; }

    }
}
