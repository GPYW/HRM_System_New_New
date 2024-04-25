using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS_Web.Models
{
    public class AttendanceManagement
    {
        [Key]
        //[Range(1,100, ErrorMessage = "Employee Id must be between 1-100")]
        [DisplayName("Employee Id:")]
        public int RecordId { get; set; }

        [Required]
        [DisplayName("Date:")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } 

        [Required]
        [DisplayName("Check-In Time:")]
        [Column(TypeName = "time")]
        public TimeSpan CheckIn { get; set; } 

        [Required]
        [DisplayName("Check-Out Time:")]
        [Column(TypeName = "time")]
        public TimeSpan CheckOut { get; set; } 

        [Required]
        [DisplayName("Break:")]
        public string Break { get; set; } 

        [Required]
        [DisplayName("Over Time:")]
        public TimeSpan OverTime { get; set; } 
    }

}
