using Microsoft.AspNetCore.Http;

namespace HRMS_Web.ViewModel
{
    public class EmployeeViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? join_date { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}
