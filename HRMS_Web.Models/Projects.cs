using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    internal class Projects
    {
        public int ProjectID { get; set; }
        public string? ProjectName { get; set;}
        
        public string? ProjectDescription { get; set; }
        public string? ProjectType { get; set;}

        public string? Project_Manager{ get; set; }

        public DateTime Start_Date { get; set; }

        public DateTime End_Date { get; set; }

        public string? Status { get; set; }

        public ICollection<Tasks> Tasks { get; set; }
    }
                           
                            
                           
                            
        
}
