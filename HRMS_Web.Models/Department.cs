using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
