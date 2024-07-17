using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class Tasks
    {
        [Key]
        [Required]
        public int TaskID { get; set; }

        [Required]
        public string? Task_Name { get; set; }

        [Required]
        public string? Task_Description { get; set; }

        [Required]
        public DateTime Due_Date { get; set; }

        [Required]
        public string? T_Priority { get; set; }

        [Required]
        public string? T_Status { get; set; }


        public string? Actions { get; set; }

        public string? TeamMember { get; set; }

        //foriegn Key
        [Required]
        public int ProjectID { get; set; }

        [ForeignKey("ProjectID")]
        // Navigation property to Project Model
        public Projects Projects { get; set; }

        //foriegn Key
        public string UserId{ get; set; }

        [ForeignKey("UserId")]

        // Navigation property to ApplicationUser Model
        public ApplicationUser? User { get; set; }
       







    }
}
