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

        public string? Task_Name { get; set; }

        public string? Project_Name { get; set; }

        public DateTime Due_Date { get; set; }

        public string? Priority { get; set; }

        public string? Status { get; set; }

        public string? Completion { get; set; }

        public string? Actions { get; set; }

        
        public int ProjectID { get; set; }

        public Projects Projects { get; set; }






    }
}
