using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Web.Models
{
    public class AttendanceManagement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }

        [Required]
        [DisplayName("Employee ID:")]
        public string EmpID { get; set; }

        [Required]
        [DisplayName("Date:")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [DisplayName("Check-In Time:")]
        [Column(TypeName = "time")]
        public TimeSpan? CheckIn { get; set; }

        [Required]
        [DisplayName("Check-Out Time:")]
        [Column(TypeName = "time")]
        public TimeSpan? CheckOut { get; set; }

        [DisplayName("Break:")]
        public string? Break { get; set; }

        [Required]
        [DisplayName("Over Time:")]
        public string? OverTime { get; set; }

        public bool IsPresent { get; set; }

        // Foreign key for ApplicationUser

        [Required]
        public string Id { get; set; }

        //Navigate property to ApplicationUser
        [ForeignKey("Id")]
        public ApplicationUser ApplicationUser { get; set; }

    }
}
