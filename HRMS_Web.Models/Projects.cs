using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class Projects
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        public string Project_Name { get; set;}

        [Required]
        public string Project_Description { get; set; }

        [Required]
        public string Client { get; set; }

        [Required]
        public string Project_Manager{ get; set; }

        [Required]
        public string Project_Team{ get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        public string P_Status { get; set; }

        [Required]
        public string P_Priority { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid rate")]
        public decimal Rate { get; set; }

        [NotMapped]
        public IFormFile FileUpload { get; set; }
        public string UploadFile { get; set; }

        //public ICollection<Tasks> Tasks { get; set; }
    }
                           
                            
                           
                            
        
}
