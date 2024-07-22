using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS_Web.Models
{
    public class Appraisal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AppraisalId { get; set; }

        [Required]
        public DateTime AppraisalDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Employee { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string Designation { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Id { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser? User { get; set; }
    }
}
