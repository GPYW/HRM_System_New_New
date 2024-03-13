using System.ComponentModel.DataAnnotations;

namespace HRMS_Web.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int MobileNo { get; set; }
        [Required]
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public DateTime join_date { get; set; }
        


    }
}
