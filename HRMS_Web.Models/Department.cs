using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HRMS_Web.Models
{
    public class Department
    {
        [Key]
        [Required]
        public string? DepartmentID { get; set; }

        [Required]
        public string? DepartmentName { get; set; }

        public int? NoOfEmployees { get; set; }

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
