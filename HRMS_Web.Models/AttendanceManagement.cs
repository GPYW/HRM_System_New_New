using System;
using System.Collections.Generic;
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
        public int RecordId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } 

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan CheckIn { get; set; } 

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan CheckOut { get; set; } 

        [Required]
        public string Break { get; set; } 

        [Required]
        public TimeSpan OverTime { get; set; } 
    }

}
