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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan CheckIn { get; set; }
        [Required]
        public TimeSpan CheckOut { get; set; }
        [Required]
        public TimeSpan Break { get; set; }
        [Required]
        public TimeSpan OverTime { get; set; }
    }
}
