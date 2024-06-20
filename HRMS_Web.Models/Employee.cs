using System.ComponentModel.DataAnnotations;

namespace HRMS_Web.Models
{
    public class Employee
    {
        [Key]
        public string? EmployeeID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MobileNo { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime DOB { get; set; }
        public DateTime join_date { get; set; }
        public string DepartmentID { get; set; }
        public Department Department { get; set; }


    }
}
