using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    internal class Tasks
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

        [Required]
        public string? Completion { get; set; }

        public string? Actions { get; set; }

        
        //public int ProjectID { get; set; }

        //public Projects Projects { get; set; }






    }
}
